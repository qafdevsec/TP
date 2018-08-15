﻿//____________________________________________________________________________
//
//  Copyright (C) 2018, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/TP
//____________________________________________________________________________

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace TP.DataStreams.Cryptography
{

  public static class CryptographyHelpers
  {
    public static (string Hex, string Base64) CalculateSHA256 (this string inputStream)
    {
      byte[] _inputStreamBytes = Encoding.UTF8.GetBytes(inputStream);
      using (SHA256 mySHA256 = SHA256Managed.Create())
      {
        byte[] hashValue = mySHA256.ComputeHash(_inputStreamBytes);
        return (hashValue.ToHexString(), Convert.ToBase64String(hashValue, Base64FormattingOptions.InsertLineBreaks));
      }
    }
    public static void EncryptData(string inFileName, string outFileName, byte[] dESKey, byte[] dESIV, IProgress<long> progress)
    {
      //Create the file streams to handle the input and output files.
      using (FileStream _inFileStream = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
      {
        using (FileStream _outFileStream = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write))
        {
          _outFileStream.SetLength(0);
          //Create variables to help with read and write.
          byte[] _buffer = new byte[100]; //This is intermediate storage for the encryption.
          long _bytesWritten = 0; //This is the total number of bytes written.
          long _inFileStreamLength = _inFileStream.Length; //This is the total length of the input file.
          int _length; //This is the number of bytes to be written at a time.
          TripleDESCryptoServiceProvider _tripleProvider = new TripleDESCryptoServiceProvider();
          using (CryptoStream _outCryptoStream = new CryptoStream(_outFileStream, _tripleProvider.CreateEncryptor(dESKey, dESIV), CryptoStreamMode.Write))
          {
            //Read from the input file, then encrypt and write to the output file.
            while (_bytesWritten < _inFileStreamLength)
            {
              _length = _inFileStream.Read(_buffer, 0, 100);
              _outCryptoStream.Write(_buffer, 0, _length);
              _bytesWritten = _bytesWritten + _length;
              progress.Report(_bytesWritten);
            }
          }
        }
      }
    }
    /// <summary>
    /// Sign and save an XML document.
    /// </summary>
    /// <param name="document">Document to be signed</param>
    /// <param name="rsa">The keys ro be used by the <see cref="RSACryptoServiceProvider"/> RSA algorithm implementation.</param>
    /// <remarks>
    /// This document cannot be verified unless the verifying code has the public key with which it was signed.
    /// </remarks>
    public static void SignSaveXml(this XmlDocument document, string rsaKeys, string fileName)
    {
      #region Check arguments
      if (document == null)
        throw new ArgumentException($"The {nameof(document)} parameter cannot be null");
      if (string.IsNullOrEmpty(rsaKeys))
        throw new ArgumentException($"The {nameof(rsaKeys)} parameter cannot be null");
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException($"The {nameof(fileName)} parameter cannot be null");
      #endregion
      using (RSACryptoServiceProvider _rsaProvider = new RSACryptoServiceProvider())
      {
        _rsaProvider.FromXmlString(rsaKeys);
        KeyInfo _keyInfo = new KeyInfo();// Add an RSAKeyValue KeyInfo (optional; helps recipient find key to validate).
        _keyInfo.AddClause(new RSAKeyValue(_rsaProvider));
        SignedXml _signedXml = new SignedXml(document)
        {
          SigningKey = _rsaProvider, // Add the key to the SignedXml document.
          KeyInfo = _keyInfo
        };
        Reference _reference = new Reference // Create a reference to be signed.
        {
          Uri = "" //An entire XML document is to be signed using an enveloped signature.
        };
        XmlDsigEnvelopedSignatureTransform _envelope = new XmlDsigEnvelopedSignatureTransform(); // Add an enveloped transformation to the reference.
        _reference.AddTransform(_envelope);
        _signedXml.AddReference(_reference); // Add the reference to the SignedXml object.
        _signedXml.ComputeSignature(); // Compute the signature.
        XmlElement _xmlDigitalSignature = _signedXml.GetXml(); // Get the XML representation of the signature and save it to an XmlElement object.
        document.DocumentElement.AppendChild(document.ImportNode(_xmlDigitalSignature, true));// Append the element to the XML document.
      }
      document.Save(fileName);
    }
    /// <summary>
    /// Load and verify the signature of an XML file against an asymmetric RSA algorithm and return the document.
    /// </summary>
    /// <param name="rsaKeys">The RSA keys as the xml document.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>An instance of the <see cref="XmlDocument"/> capturing the xml file.</returns>
    /// <exception cref="ArgumentException">
    /// rsaKeys
    /// or
    /// fileName
    /// </exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
    /// <remarks>There must be only one signature.</remarks>
    public static XmlDocument LoadVerifyXml(string rsaKeys, string fileName)
    {
      #region Check arguments
      if (string.IsNullOrEmpty(rsaKeys))
        throw new ArgumentException($"The {nameof(rsaKeys)} parameter cannot be null");
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException($"The {nameof(fileName)} parameter cannot be null");
      #endregion
      (XmlDocument _document, SignedXml _signedXml) = LoadsIGNEDXmlFile(fileName);
      using (RSACryptoServiceProvider _provider = new RSACryptoServiceProvider())
      {
        _provider.FromXmlString(rsaKeys);
        if (!_signedXml.CheckSignature(_provider))// Check the signature and return the result.
          throw new CryptographicException($"Wrong signature of the document.");
      }
      return _document;
    }
    public static XmlDocument LoadVerifyXml(string fileName)
    {
      #region Check arguments
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException($"The {nameof(fileName)} parameter cannot be null");
      #endregion
      (XmlDocument _document, SignedXml _signedXml) = LoadsIGNEDXmlFile(fileName);
      if (!_signedXml.CheckSignature())// Check the signature and return the result.
        throw new System.Security.Cryptography.CryptographicException($"Wrong signature of the document.");
      return _document;
    }
    private static string ToHexString(this byte[] bytes)
    {
      char[] c = new char[bytes.Length * 2];
      int b;
      for (int i = 0; i < bytes.Length; i++)
      {
        b = bytes[i] >> 4;
        c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
        b = bytes[i] & 0xF;
        c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
      }
      return new string(c);
    }
    private static (XmlDocument document, SignedXml signedXml) LoadsIGNEDXmlFile(string fileName)
    {
      XmlDocument _document = new XmlDocument();
      _document.Load(fileName);
      SignedXml _signedXml = new SignedXml(_document);
      XmlNodeList _nodeList = _document.GetElementsByTagName("Signature");// Find the "Signature" node and create a new XmlNodeList object.
      if (_nodeList.Count != 1) // There must be only one signature. Return false if 0 or more than one signatures was found.
        throw new CryptographicException($"Expected exactly one signature but the file contaons {_nodeList.Count}.");
      _signedXml.LoadXml((XmlElement)_nodeList[0]);// Load the first <signature> node.
      return (_document, _signedXml);
    }
  }
}