﻿//MIT, 2019-present, WinterDev
using System;
using System.IO;

namespace Typography.OpenFont.Tables
{
    //from https://docs.microsoft.com/en-us/typography/opentype/spec/cbdt

    //Table structure

    //The CBDT table is used to embed color bitmap glyph data. It is used together with the CBLC table,
    //which provides embedded bitmap locators.
    //The formats of these two tables are backward compatible with the EBDT and EBLC tables
    //used for embedded monochrome and grayscale bitmaps.

    //The CBDT table begins with a header containing simply the table version number.
    //Type 	    Name 	        Description
    //uint16 	majorVersion 	Major version of the CBDT table, = 3.
    //uint16 	minorVersion 	Minor version of the CBDT table, = 0.

    //Note that the first version of the CBDT table is 3.0.

    //The rest of the CBDT table is a collection of bitmap data.
    //The data can be presented in three possible formats,
    //indicated by information in the CBLC table.
    //Some of the formats contain metric information plus image data, 
    //and other formats contain only the image data. Long word alignment is not required for these subtables;
    //byte alignment is sufficient.
    class CBDT : TableEntry
    {
        public const string _N = "CBDT";
        public override string Name => _N;

        protected override void ReadContentFrom(BinaryReader reader)
        {
            ushort majorVersion = reader.ReadUInt16();
            ushort minorVersion = reader.ReadUInt16();
            
        }
    }
}