﻿using System;
using System.Diagnostics;
using System.IO;

namespace Brutal.Dev.StrongNameSigner
{
  /// <summary>
  /// Utility class that assists in the handling of temporary files during assembly signing. It will create
  /// a temporary directory to hold generated files during the signing process, and will move these to their
  /// final location upon calling <see cref="Commit"/>. It also ensures that all temporary/intermediate files
  /// are deleted upon disposing the instance.
  /// </summary>
  /// <seealso cref="System.IDisposable" />
  internal sealed class OutputFileManager : IDisposable
  {
    private string tempDir;

    public OutputFileManager(string sourceAssemblyPath, string targetAssemblyPath)
    {
      SourceAssemblyPath = Path.GetFullPath(sourceAssemblyPath);
      TargetAssemblyPath = Path.GetFullPath(targetAssemblyPath);
      IsInPlaceReplace = string.Equals(SourceAssemblyPath, TargetAssemblyPath, StringComparison.Ordinal);

      if (IsInPlaceReplace)
      {
        tempDir = Path.Combine(Path.GetTempPath(), $"StrongNamerTemp.{Process.GetCurrentProcess().Id}.{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        IntermediateAssemblyPath = Path.Combine(tempDir, Path.GetFileName(sourceAssemblyPath));
      }
      else
      {
        IntermediateAssemblyPath = TargetAssemblyPath;
      }

      HasSymbols = File.Exists(Path.ChangeExtension(SourceAssemblyPath, ".pdb"));
    }

    ~OutputFileManager()
    {
      Dispose();
    }

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the source assembly has a matching PDB file.
    /// </summary>
    public bool HasSymbols { get; }

    /// <summary>
    /// Indicates whether the SourceAssemblyPath and the TargetAssemblyPath are equal.
    /// </summary>
    public bool IsInPlaceReplace { get; }

    /// <summary>
    /// Gets the path of the source assembly.
    /// </summary>
    public string SourceAssemblyPath { get; }

    /// <summary>
    /// Gets the path to the .PDB file of the source assembly. (Does not check for existence of the file)
    /// </summary>
    public string SourcePdbPath => Path.ChangeExtension(SourceAssemblyPath, ".pdb");

    /// <summary>
    /// Gets the intermediate path to which the new assembly should be written. This may be the same as <see cref="TargetAssemblyPath"/>,
    /// if it is different from <see cref="SourceAssemblyPath"/>. (This property is never equal to <see cref="SourceAssemblyPath"/>).
    /// </summary>
    public string IntermediateAssemblyPath { get; }

    /// <summary>
    /// Gets the intermediate path to which the new .PDB should be written. This may be the same as <see cref="TargetAssemblyPath"/>,
    /// if it is different from <see cref="SourceAssemblyPath"/>.
    /// </summary>
    public string IntermediatePdbPath => Path.ChangeExtension(IntermediateAssemblyPath, ".pdb");

    /// <summary>
    /// Gets the path to where the final output assembly should reside. This may be the same as <see cref="SourceAssemblyPath"/>.
    /// </summary>
    public string TargetAssemblyPath { get; }

    /// <summary>
    /// Gets the path to the .PDB file of the target assembly. (Does not check for existence of the file)
    /// </summary>
    public string TargetPdbPath => Path.ChangeExtension(TargetAssemblyPath, ".pdb");

    /// <summary>
    /// Gets the path to where a backup of the source assembly should be saved.
    /// </summary>
    public string BackupAssemblyPath => SourceAssemblyPath + ".unsigned";

    /// <summary>
    /// Gets the path to where a backup of the source .PDB file should be saved.
    /// </summary>
    public string BackupPdbPath => SourcePdbPath + ".unsigned";

    /// <summary>
    /// This property will be set to true after <see cref="CreateBackup"/> has been called.
    /// </summary>
    public bool HasBackup { get; private set; }

    private bool UseTemporaryDirectory => tempDir != null;

    #endregion

    /// <summary>
    /// Creates a backup of the input files by simply copying them to the <see cref="BackupAssemblyPath"/> and <see cref="BackupPdbPath"/>.
    /// </summary>
    public void CreateBackup()
    {
      CopyFile(SourceAssemblyPath, BackupAssemblyPath);

      if (HasSymbols)
      {
        CopyFile(SourcePdbPath, BackupPdbPath);
      }

      HasBackup = true;
    }

    /// <summary>
    /// Directly copies <see cref="SourceAssemblyPath"/> and <see cref="SourcePdbPath"/> to <see cref="TargetAssemblyPath"/>
    /// and <see cref="TargetPdbPath"/> (if the source files exists).
    /// </summary>
    public void CopySourceToFinalOutput()
    {
      CopyFile(SourceAssemblyPath, TargetAssemblyPath);

      if (HasSymbols)
      {
        CopyFile(SourcePdbPath, TargetPdbPath);
      }
    }

    /// <summary>
    /// Moves the intermediate files to the target locations if a temporary directory was used during generation.
    /// Otherwise this method does nothing.
    /// </summary>
    public void Commit()
    {
      if (UseTemporaryDirectory)
      {
        try
        {
          CopyFile(IntermediateAssemblyPath, TargetAssemblyPath);

          if (HasSymbols)
          {
            CopyFile(IntermediatePdbPath, TargetPdbPath);
          }

          if (File.Exists(IntermediateAssemblyPath))
          {
            File.Delete(IntermediateAssemblyPath);
          }
        }
        catch (Exception ex)
        {
          SigningHelper.Log(ex.ToString());

          if (HasBackup)
          {
            // Restore backup
            CopyFile(BackupAssemblyPath, SourceAssemblyPath);
            CopyFile(BackupPdbPath, SourcePdbPath);

            if (File.Exists(BackupAssemblyPath))
            {
              File.Delete(BackupAssemblyPath);
            }

            if (File.Exists(BackupPdbPath))
            {
              File.Delete(BackupPdbPath);
            }
          }

          throw;
        }
      }
    }

    public void Dispose()
    {
      if (tempDir != null)
      {
        try
        {
          Directory.Delete(tempDir, true);
          tempDir = null;
        }
        catch (Exception ex)
        {
          // Ignore errors when attempting to clean up temporary directory.
          SigningHelper.Log(ex.ToString());
        }
      }

      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Copies or moves a single file if it exists. It will always overwrite the target if it exists (provided that
    /// the source file exists).
    /// </summary>
    /// <param name="source">The source file to copy/move if it exists.</param>
    /// <param name="target">The target file to write to.</param>
    private static void CopyFile(string source, string target)
    {
      try
      {
        if (File.Exists(source))
        {
          if (File.Exists(target))
          {
            File.SetAttributes(target, FileAttributes.Normal);
          }

          File.Copy(source, target, true);
        }
      }
      catch (Exception ex)
      {
        // Ignore file operation failures.
        SigningHelper.Log(ex.ToString());
      }
    }
  }
}
