﻿//MIT, 2016-2017, WinterDev

using Typography.OpenFont;
namespace Typography.Rendering
{
    //-----------------------------------
    //sample GlyphPathBuilder :
    //for your flexiblity of glyph path builder.
    //-----------------------------------

    public class GlyphPathBuilder
    {
        readonly Typeface _typeface;
        TrueTypeInterpreter _trueTypeInterpreter;
        GlyphAutoFit _autoFit = new GlyphAutoFit();
        GlyphPointF[] _outputGlyphPoints;
        ushort[] _outputContours;
        float _recentPixelScale;
        bool _useInterpreter;
        bool _useAutoHint;

        public GlyphPathBuilder(Typeface typeface)
        {
            _typeface = typeface;
            this.UseTrueTypeInstructions = false;//default?
            _trueTypeInterpreter = new TrueTypeInterpreter();
            _trueTypeInterpreter.SetTypeFace(typeface);
            _recentPixelScale = 1;
        }
        public Typeface Typeface { get { return _typeface; } }

        /// <summary>
        /// specific output glyph size (in points)
        /// </summary>
        public float SizeInPoints
        {
            get;
            private set;
        }
        /// <summary>
        /// use Maxim's Agg Vertical Hinting
        /// </summary>
        public bool UseVerticalHinting { get; set; }
        /// <summary>
        /// process glyph with true type instructions
        /// </summary>
        public bool UseTrueTypeInstructions
        {
            get { return _useInterpreter; }
            set
            {
                _useInterpreter = value;
            }
        }

        public bool MinorAdjustFitYForAutoFit
        {
            get { return this._autoFit.HalfPixel; }
            set { this._autoFit.HalfPixel = value; }
        }
        public void Build(char c, float sizeInPoints)
        {
            BuildFromGlyphIndex((ushort)_typeface.LookupIndex(c), sizeInPoints);
        }
        public void BuildFromGlyphIndex(ushort glyphIndex, float sizeInPoints)
        {
            this.SizeInPoints = sizeInPoints;
            Build(glyphIndex, _typeface.GetGlyphByIndex(glyphIndex));
        }

        void Build(ushort glyphIndex, Glyph glyph)
        {
            //-------------------------------------------
            //1. start with original points/contours from glyph
            this._outputGlyphPoints = glyph.GlyphPoints;
            this._outputContours = glyph.EndPoints;
            //-------------------------------------------              
            Typeface currentTypeFace = this._typeface;
            _recentPixelScale = currentTypeFace.CalculateFromPointToPixelScale(SizeInPoints); //***
            _useAutoHint = false;//reset             
            //-------------------------------------------  
            //2. process glyph points
            if (UseTrueTypeInstructions &&
                currentTypeFace.HasPrepProgramBuffer &&
                glyph.HasGlyphInstructions)
            {
                _trueTypeInterpreter.UseVerticalHinting = this.UseVerticalHinting;
                //output as points
                this._outputGlyphPoints = _trueTypeInterpreter.HintGlyph(glyphIndex, SizeInPoints);
                _recentPixelScale = 1;
            }
            else
            {

                //not use interperter so we need to scale it with our machnism
                //this demonstrate our auto hint engine ***
                //you can change this to your own hint engine***  
                if (this.UseVerticalHinting)
                {
                    _useAutoHint = true;

                    //1. autofit 
                    _autoFit.Hint(
                        this.GetOutputPoints(),
                        this.GetOutputContours(), this.GetPixelScale());
                    _recentPixelScale = 1;
                }
            }
        }
        public void ReadShapes(IGlyphTranslator tx)
        {
            if (_useAutoHint)
            {
                //read from our auto hint
                _autoFit.ReadOutput(tx);
            }
            else
            {
                tx.Read(this._outputGlyphPoints, this._outputContours);
            }
        }

        public float GetPixelScale()
        {
            return _recentPixelScale;
        }
        public GlyphPointF[] GetOutputPoints()
        {
            return this._outputGlyphPoints;
        }
        public ushort[] GetOutputContours()
        {
            return this._outputContours;
        }
    }

}