﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2023, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//  by introducing yourself and telling us what you do with this community.
//_____________________________________________________________________________________________________________________________________

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TPA.Reflection.CodeGeneration;

namespace TPA.Reflection.UnitTest.CodeGeneration
{
  [TestClass]
  public class CSharpCodeFactoryUnitTest
  {
    [TestCleanup]
    public void TestCleanupMethod()
    {
      if (File.Exists(_outputFileName))
        File.Delete(_outputFileName);
    }

    [TestMethod]
    public void GenerateCSharpCodeTest()
    {
      Assert.IsFalse(File.Exists(_outputFileName));
      /// <summary>
      /// Create the CodeDOM graph and generate the code.
      /// </summary>
      CSharpCodeFactory _sample = new CSharpCodeFactory();
      _sample.AddFields();
      _sample.AddProperties();
      _sample.AddMethod();
      _sample.AddConstructor();
      _sample.AddEntryPoint();
      _sample.GenerateCSharpCode(_outputFileName);
      FileInfo _createdFile = new FileInfo(_outputFileName);
      Assert.IsTrue(_createdFile.Exists);
      using (StreamReader _reader = new StreamReader(_createdFile.Name))
        Console.Write(_reader.ReadToEnd());
    }

    /// <summary>
    /// The name of the file to contain the source code.
    /// </summary>
    private const string _outputFileName = "SampleCode.cs";
  }
}