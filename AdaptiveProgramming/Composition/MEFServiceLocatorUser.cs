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

using System.ComponentModel.Composition;

namespace TPA.Composition
{
  public class MEFServiceLocatorUser
  {
    public void DataProcessing()
    {
      if (Logger != null)
        Logger.Log("Executing DataProcessingWithSimpleLog");
    }

    [Import(typeof(ILogger))]
    public ILogger Logger { get; set; }
  }
}