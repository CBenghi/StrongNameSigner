﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Brutal.Dev.StrongNameSigner.Tests
{
  public class SignAssemblyTests
  {
    private static readonly string TestAssemblyDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAssemblies");

    [Fact]
    public void SignAssembly_Public_API_Test()
    {
      var info = SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET20.exe"), string.Empty, Path.Combine(TestAssemblyDirectory, "Signed"));
      info.ShouldNotBe(null);
      info.DotNetVersion.ShouldBe("2.0.50727");
      info.Is32BitOnly.ShouldBe(false);
      info.Is32BitPreferred.ShouldBe(false);
      info.Is64BitOnly.ShouldBe(false);
      info.IsAnyCpu.ShouldBe(true);
      info.IsSigned.ShouldBe(true);
    }

    [Fact]
    public void SignAssembly_Public_API_Invalid_Path_Should_Throw_Exception()
    {
      Action act = () => SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET20.doesnotexist")).Dispose();
      act.ShouldThrow<FileNotFoundException>();
    }

    [Fact]
    public void SignAssembly_Public_API_Password_Protected_Key_Path_Should_Throw_Exception_When_Password_Not_Provided()
    {
      Action act = () => SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET20.exe"), Path.Combine(TestAssemblyDirectory, "PasswordTest.pfx")).Dispose();
      act.ShouldThrow<CryptographicException>();
    }

    [Fact]
    public void SignAssembly_Public_API_Password_Protected_Key_Path_Should_Throw_Exception_When_Wrong_Password_Provided()
    {
      Action act = () => SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET20.exe"), Path.Combine(TestAssemblyDirectory, "PasswordTest.pfx"), Path.Combine(TestAssemblyDirectory, "Signed"), "oops").Dispose();
      act.ShouldThrow<CryptographicException>();
    }

    [Fact]
    public void SignAssembly_Public_API_Password_Protected_Key_Path_Should_Work_With_Correct_Password()
    {
      Action act = () => SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET20.exe"), Path.Combine(TestAssemblyDirectory, "PasswordTest.pfx"), Path.Combine(TestAssemblyDirectory, "Signed"), "password123").Dispose();
      act.ShouldNotThrow();
    }

    [Fact]
    public void SignAssembly_Public_API_Invalid_Key_Path_Should_Throw_Exception_For_Missing_Directory()
    {
      Action act = () => SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET40.exe"), "C:\\DoesNotExist\\KeyFile.snk");
      act.ShouldThrow<DirectoryNotFoundException>();
    }

    [Fact]
    public void SignAssembly_Public_API_Invalid_Key_Path_Should_Throw_Exception_For_Missing_File()
    {
      Action act = () => SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET20.exe"), "C:\\KeyFileThatDoesNotExist.snk");
      act.ShouldThrow<FileNotFoundException>();
    }

    [Fact]
    public void SignAssembly_Should_Reassemble_NET_20_Assembly_Correctly()
    {
      using (var info = SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET20.exe"), string.Empty, Path.Combine(TestAssemblyDirectory, "Signed")))
      {
        info.DotNetVersion.ShouldBe("2.0.50727");
        info.IsAnyCpu.ShouldBe(true);
        info.Is32BitOnly.ShouldBe(false);
        info.Is32BitPreferred.ShouldBe(false);
        info.Is64BitOnly.ShouldBe(false);
        info.IsSigned.ShouldBe(true);
      }
    }

    [Fact]
    public void SignAssembly_Should_Reassemble_NET_40_Assembly_Correctly()
    {
      using (var info = SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET40.exe"), string.Empty, Path.Combine(TestAssemblyDirectory, "Signed")))
      {
        info.DotNetVersion.ShouldBe("4.0.30319");
        info.IsAnyCpu.ShouldBe(true);
        info.Is32BitOnly.ShouldBe(false);
        info.Is32BitPreferred.ShouldBe(false);
        info.Is64BitOnly.ShouldBe(false);
        info.IsSigned.ShouldBe(true);
      }
    }

    [Fact]
    public void SignAssembly_Should_Reassemble_NET_45_Assembly_Correctly()
    {
      using (var info = SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET45.exe"), string.Empty, Path.Combine(TestAssemblyDirectory, "Signed")))
      {
        info.DotNetVersion.ShouldBe("4.0.30319");
        info.IsAnyCpu.ShouldBe(true);
        info.Is32BitOnly.ShouldBe(false);
        info.Is32BitPreferred.ShouldBe(true);
        info.Is64BitOnly.ShouldBe(false);
        info.IsSigned.ShouldBe(true);
      }
    }

    [Fact]
    public void SignAssembly_Should_Reassemble_NET_20_x86_Assembly_Correctly()
    {
      using (var info = SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET20-x86.exe"), string.Empty, Path.Combine(TestAssemblyDirectory, "Signed")))
      {
        info.DotNetVersion.ShouldBe("2.0.50727");
        info.IsAnyCpu.ShouldBe(false);
        info.Is32BitOnly.ShouldBe(true);
        info.Is64BitOnly.ShouldBe(false);
        info.IsSigned.ShouldBe(true);
      }
    }

    [Fact]
    public void SignAssembly_Should_Reassemble_NET_40_x86_Assembly_Correctly()
    {
      using (var info = SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET40-x86.exe"), string.Empty, Path.Combine(TestAssemblyDirectory, "Signed")))
      {
        info.DotNetVersion.ShouldBe("4.0.30319");
        info.IsAnyCpu.ShouldBe(false);
        info.Is32BitOnly.ShouldBe(true);
        info.Is32BitPreferred.ShouldBe(false);
        info.Is64BitOnly.ShouldBe(false);
        info.IsSigned.ShouldBe(true);
      }
    }

    [Fact]
    public void SignAssembly_Should_Reassemble_NET_20_x64_Assembly_Correctly()
    {
      using (var info = SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET20-x64.exe"), string.Empty, Path.Combine(TestAssemblyDirectory, "Signed")))
      {
        info.DotNetVersion.ShouldBe("2.0.50727");
        info.IsAnyCpu.ShouldBe(false);
        info.Is32BitOnly.ShouldBe(false);
        info.Is32BitPreferred.ShouldBe(false);
        info.Is64BitOnly.ShouldBe(true);
        info.IsSigned.ShouldBe(true);
      }
    }

    [Fact]
    public void SignAssembly_Should_Reassemble_NET_40_x64_Assembly_Correctly()
    {
      using (var info = SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.NET40-x64.exe"), string.Empty, Path.Combine(TestAssemblyDirectory, "Signed")))
      {
        info.DotNetVersion.ShouldBe("4.0.30319");
        info.IsAnyCpu.ShouldBe(false);
        info.Is32BitOnly.ShouldBe(false);
        info.Is32BitPreferred.ShouldBe(false);
        info.Is64BitOnly.ShouldBe(true);
        info.IsSigned.ShouldBe(true);
      }
    }

    [Fact]
    public void SignAssembly_Should_Reassemble_Core_5_Assembly_Correctly()
    {
      using (var info = SigningHelper.SignAssembly(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.Core5.dll"), string.Empty, Path.Combine(TestAssemblyDirectory, "Signed")))
      {
        info.DotNetVersion.ShouldBe("4.0.30319");
        info.IsAnyCpu.ShouldBe(true);
        info.Is32BitOnly.ShouldBe(false);
        info.Is32BitPreferred.ShouldBe(false);
        info.Is64BitOnly.ShouldBe(false);
        info.IsSigned.ShouldBe(true);
      }
    }

    [Fact]
    public void SignAssembly_InPlaceWithPdb_Should_Succeed()
    {
      var tempDir = Path.Combine(TestAssemblyDirectory, Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(tempDir);
      try
      {
        string targetAssemblyPath = Path.Combine(tempDir, "Brutal.Dev.StrongNameSigner.TestAssembly.A.dll");
        File.Copy(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.A.dll"), targetAssemblyPath);
        File.Copy(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.A.pdb"), Path.Combine(tempDir, "Brutal.Dev.StrongNameSigner.TestAssembly.A.pdb"));

        using (var info = SigningHelper.SignAssembly(targetAssemblyPath))
        {
          info.IsSigned.ShouldBeTrue();
        }
      }
      finally
      {
        Directory.Delete(tempDir, true);
      }
    }

    [Fact]
    public void SignAssembly_NewLocationWithPdb_Should_Succeed()
    {
      var tempDir = Path.Combine(TestAssemblyDirectory, Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(tempDir);
      var outDir = Path.Combine(tempDir, "out");
      Directory.CreateDirectory(outDir);
      try
      {
        string sourceAssemblyPath = Path.Combine(tempDir, "Brutal.Dev.StrongNameSigner.TestAssembly.A.dll");
        File.Copy(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.A.dll"), sourceAssemblyPath);
        File.Copy(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.A.pdb"), Path.Combine(tempDir, "Brutal.Dev.StrongNameSigner.TestAssembly.A.pdb"));

        using (var info = SigningHelper.SignAssembly(sourceAssemblyPath, null, outDir))
        {
          string outAssembly = Path.Combine(outDir, Path.GetFileName(sourceAssemblyPath));
          File.Exists(outAssembly).ShouldBeTrue();
          File.Exists(Path.ChangeExtension(outAssembly, ".pdb")).ShouldBeTrue();
          info.IsSigned.ShouldBeTrue();
        }
      }
      finally
      {
        Directory.Delete(tempDir, true);
      }
    }

    [Fact]
    public void SignAssembly_NewLocationWithoutPdb_Should_Succeed()
    {
      var tempDir = Path.Combine(TestAssemblyDirectory, Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(tempDir);
      var outDir = Path.Combine(tempDir, "out");
      Directory.CreateDirectory(outDir);
      try
      {
        string sourceAssemblyPath = Path.Combine(tempDir, "Brutal.Dev.StrongNameSigner.TestAssembly.A.dll");
        File.Copy(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.A.dll"), sourceAssemblyPath);

        using (var info = SigningHelper.SignAssembly(sourceAssemblyPath, null, outDir))
        {
          string outAssembly = Path.Combine(outDir, Path.GetFileName(sourceAssemblyPath));
          File.Exists(outAssembly).ShouldBeTrue();
          info.IsSigned.ShouldBeTrue();
        }
      }
      finally
      {
        Directory.Delete(tempDir, true);
      }
    }

    [Fact]
    public void SignAssembly_InPlaceWithoutPdb_Should_Succeed()
    {
      var tempDir = Path.Combine(TestAssemblyDirectory, Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(tempDir);
      try
      {
        string targetAssemblyPath = Path.Combine(tempDir, "Brutal.Dev.StrongNameSigner.TestAssembly.A.dll");
        File.Copy(Path.Combine(TestAssemblyDirectory, "Brutal.Dev.StrongNameSigner.TestAssembly.A.dll"), targetAssemblyPath);

        using (var info = SigningHelper.SignAssembly(targetAssemblyPath))
        {
          info.IsSigned.ShouldBeTrue();
        }
      }
      finally
      {
        Directory.Delete(tempDir, true);
      }
    }

    [Fact]
#pragma warning disable S2699 // Tests should include assertions
    public void SignAssembly_InParralel_Should_Succeed()
    {
      Parallel.Invoke(
        SignAssembly_Public_API_Test,
        SignAssembly_Should_Reassemble_NET_45_Assembly_Correctly
      );
    }
#pragma warning restore S2699 // Tests should include assertions
  }
}
