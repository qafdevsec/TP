﻿//__________________________________________________________________________________________
//
//  Copyright 2022 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//  with an introduction of yourself and tell us about what you do with this community.
//__________________________________________________________________________________________

using System.Diagnostics;

namespace TP.InformationComputation.LayersCommunication.Logic
{
  internal abstract class ConstructorInjection : ILogic
  {
    public ConstructorInjection(ITraceSource traceEngine)
    {
      m_TraceEngine = traceEngine ?? throw new ArgumentNullException(nameof(traceEngine));
    }

    #region ILogic

    public void Alpha()
    {
      m_TraceEngine.TraceData(TraceEventType.Verbose, nameof(Alpha).GetHashCode(), "Entering Alpha");
    }

    public void Bravo()
    {
      m_TraceEngine.TraceData(TraceEventType.Verbose, nameof(Bravo).GetHashCode(), "Entering Bravo");
    }

    public void Charlie()
    {
      m_TraceEngine.TraceData(TraceEventType.Verbose, nameof(Charlie).GetHashCode(), "Entering Charlie");
    }

    public void Delta()
    {
      m_TraceEngine.TraceData(TraceEventType.Verbose, nameof(Delta).GetHashCode(), "Entering Delta");
    }

    #endregion ILogic

    private ITraceSource m_TraceEngine;
  }
}