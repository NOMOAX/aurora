using System.Text.RegularExpressions;

namespace Aurora
{
    /// <summary>
    /// Constant and static read-only values.
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// String constants.
        /// </summary>
        public static class String
        {
            /// <summary>
            /// The author's name.
            /// </summary>
            public const string AuthorName = "谢凯文";

            /// <summary>
            /// The author's English name.
            /// </summary>
            public const string AuthorNameEnglish = "Kevin Xie";

            /// <summary>
            /// A format that allows single-precision floating-point values to round-trip successfully.
            /// </summary>
            public const string FloatFormatRoundTrip = "G9";

            /// <summary>
            /// A format that obtains significant fixed-point digits for single-precision floating-point values.
            /// </summary>
            public const string FloatFormatSignificantFixedPointFigures =
                "0.###################################################";

            /// <summary>
            /// A format that allows double-precision floating-point values to round-trip successfully.
            /// </summary>
            public const string DoubleFormatRoundTrip = "G17";

            /// <summary>
            /// A format that obtains significant fixed-point digits for double-precision floating-point values.
            /// </summary>
            public const string DoubleFormatSignificantFixedPointFigures =
                "0.##################################################################################################################################################################################################################################################################################################################################################";

            /// <summary>
            /// A regular expression pattern that matches email addresses conforming to the RFC 5322 standard.
            /// </summary>
            public const string EmailAddressRegexPattern =
                "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";
        }

        /// <summary>
        /// Static read-only regular expressions.
        /// </summary>
        public static class Regex
        {
            /// <summary>
            /// A pattern that matches email addresses conforming to the RFC 5322 standard.
            /// </summary>
            public static readonly System.Text.RegularExpressions.Regex EmailAddressRegex = new(
                String.EmailAddressRegexPattern,
                RegexOptions.Compiled
            );
        }

        /// <summary>
        /// Static read-only time intervals.
        /// </summary>
        public static class TimeSpan
        {
            /// <summary>
            /// The maximum timeout interval supported by the timer.
            /// </summary>
            public static readonly System.TimeSpan TimerMaxSupportedTimeout =
                System.TimeSpan.FromMilliseconds(4294967294);
        }

        /// <summary>
        /// Character constants.
        /// </summary>
        public static class Character
        {
            #region Basic Latin ('\u0000' → '\u007f')

            /// <summary>
            /// Null.
            /// </summary>
            /// <remarks>
            /// Decimal:0
            /// <br/>
            /// Hex:'\u0000'
            /// <br/>
            /// Escape:'\0'
            /// </remarks>
            public const char Null = '\u0000';

            /// <summary>
            /// Start of Heading.
            /// </summary>
            /// <remarks>
            /// Decimal:1
            /// <br/>
            /// Hex:'\u0001'
            /// </remarks>
            public const char StartOfHeading = '\u0001';

            /// <summary>
            /// Start of Text.
            /// </summary>
            /// <remarks>
            /// Decimal:2
            /// <br/>
            /// Hex:'\u0002'
            /// </remarks>
            public const char StartOfText = '\u0002';

            /// <summary>
            /// End of Text.
            /// </summary>
            /// <remarks>
            /// Decimal:3
            /// <br/>
            /// Hex:'\u0003'
            /// </remarks>
            public const char EndOfText = '\u0003';

            /// <summary>
            /// End of Transmission.
            /// </summary>
            /// <remarks>
            /// Decimal:4
            /// <br/>
            /// Hex:'\u0004'
            /// </remarks>
            public const char EndOfTransmission = '\u0004';

            /// <summary>
            /// Enquiry.
            /// </summary>
            /// <remarks>
            /// Decimal:5
            /// <br/>
            /// Hex:'\u0005'
            /// </remarks>
            public const char Enquiry = '\u0005';

            /// <summary>
            /// Acknowledge.
            /// </summary>
            /// <remarks>
            /// Decimal:6
            /// <br/>
            /// Hex:'\u0006'
            /// </remarks>
            public const char Acknowledge = '\u0006';

            /// <summary>
            /// Bell.
            /// </summary>
            /// <remarks>
            /// Decimal:7
            /// <br/>
            /// Hex:'\u0007'
            /// <br/>
            /// Escape:'\a'
            /// </remarks>
            public const char Bell = '\u0007';

            /// <summary>
            /// Backspace.
            /// </summary>
            /// <remarks>
            /// Decimal:8
            /// <br/>
            /// Hex:'\u0008'
            /// <br/>
            /// Escape:'\b'
            /// </remarks>
            public const char Backspace = '\u0008';

            /// <summary>
            /// Horizontal Tabulation.
            /// </summary>
            /// <remarks>
            /// Decimal:9
            /// <br/>
            /// Hex:'\u0009'
            /// <br/>
            /// Escape:'\t'
            /// </remarks>
            public const char HorizontalTabulation = '\u0009';

            /// <summary>
            /// New Line.
            /// </summary>
            /// <remarks>
            /// Decimal:10
            /// <br/>
            /// Hex:'\u000a'
            /// <br/>
            /// Escape:'\n'
            /// </remarks>
            public const char NewLine = '\u000a';

            /// <summary>
            /// Vertical Tabulation.
            /// </summary>
            /// <remarks>
            /// Decimal:11
            /// <br/>
            /// Hex:'\u000b'
            /// <br/>
            /// Escape:'\v'
            /// </remarks>
            public const char VerticalTabulation = '\u000b';

            /// <summary>
            /// Form Feed.
            /// </summary>
            /// <remarks>
            /// Decimal:12
            /// <br/>
            /// Hex:'\u000c'
            /// <br/>
            /// Escape:'\f'
            /// </remarks>
            public const char FormFeed = '\u000c';

            /// <summary>
            /// Carriage Return.
            /// </summary>
            /// <remarks>
            /// Decimal:13
            /// <br/>
            /// Hex:'\u000d'
            /// <br/>
            /// Escape:'\r'
            /// </remarks>
            public const char CarriageReturn = '\u000d';

            /// <summary>
            /// Shift Out.
            /// </summary>
            /// <remarks>
            /// Decimal:14
            /// <br/>
            /// Hex:'\u000e'
            /// </remarks>
            public const char ShiftOut = '\u000e';

            /// <summary>
            /// Shift In.
            /// </summary>
            /// <remarks>
            /// Decimal:15
            /// <br/>
            /// Hex:'\u000f'
            /// </remarks>
            public const char ShiftIn = '\u000f';

            /// <summary>
            /// Data Link Escape.
            /// </summary>
            /// <remarks>
            /// Decimal:16
            /// <br/>
            /// Hex:'\u0010'
            /// </remarks>
            public const char DataLinkEscape = '\u0010';

            /// <summary>
            /// Device Control One.
            /// </summary>
            /// <remarks>
            /// Decimal:17
            /// <br/>
            /// Hex:'\u0011'
            /// </remarks>
            public const char DeviceControlOne = '\u0011';

            /// <summary>
            /// Device Control Two.
            /// </summary>
            /// <remarks>
            /// Decimal:18
            /// <br/>
            /// Hex:'\u0012'
            /// </remarks>
            public const char DeviceControlTwo = '\u0012';

            /// <summary>
            /// Device Control Three.
            /// </summary>
            /// <remarks>
            /// Decimal:19
            /// <br/>
            /// Hex:'\u0013'
            /// </remarks>
            public const char DeviceControlThree = '\u0013';

            /// <summary>
            /// Device Control Four.
            /// </summary>
            /// <remarks>
            /// Decimal:20
            /// <br/>
            /// Hex:'\u0014'
            /// </remarks>
            public const char DeviceControlFour = '\u0014';

            /// <summary>
            /// Negative Acknowledge.
            /// </summary>
            /// <remarks>
            /// Decimal:21
            /// <br/>
            /// Hex:'\u0015'
            /// </remarks>
            public const char NegativeAcknowledge = '\u0015';

            /// <summary>
            /// Synchronous Idle.
            /// </summary>
            /// <remarks>
            /// Decimal:22
            /// <br/>
            /// Hex:'\u0016'
            /// </remarks>
            public const char SynchronousIdle = '\u0016';

            /// <summary>
            /// End of Transmission Block.
            /// </summary>
            /// <remarks>
            /// Decimal:23
            /// <br/>
            /// Hex:'\u0017'
            /// </remarks>
            public const char EndOfTransmissionBlock = '\u0017';

            /// <summary>
            /// Cancel.
            /// </summary>
            /// <remarks>
            /// Decimal:24
            /// <br/>
            /// Hex:'\u0018'
            /// </remarks>
            public const char Cancel = '\u0018';

            /// <summary>
            /// End of Medium.
            /// </summary>
            /// <remarks>
            /// Decimal:25
            /// <br/>
            /// Hex:'\u0019'
            /// </remarks>
            public const char EndOfMedium = '\u0019';

            /// <summary>
            /// Substitute.
            /// </summary>
            /// <remarks>
            /// Decimal:26
            /// <br/>
            /// Hex:'\u001a'
            /// </remarks>
            public const char Substitute = '\u001a';

            /// <summary>
            /// Escape.
            /// </summary>
            /// <remarks>
            /// Decimal:27
            /// <br/>
            /// Hex:'\u001b'
            /// </remarks>
            public const char Escape = '\u001b';

            /// <summary>
            /// File Separator.
            /// </summary>
            /// <remarks>
            /// Decimal:28
            /// <br/>
            /// Hex:'\u001c'
            /// </remarks>
            public const char FileSeparator = '\u001c';

            /// <summary>
            /// Group Separator.
            /// </summary>
            /// <remarks>
            /// Decimal:29
            /// <br/>
            /// Hex:'\u001d'
            /// </remarks>
            public const char GroupSeparator = '\u001d';

            /// <summary>
            /// Record Separator.
            /// </summary>
            /// <remarks>
            /// Decimal:30
            /// <br/>
            /// Hex:'\u001e'
            /// </remarks>
            public const char RecordSeparator = '\u001e';

            /// <summary>
            /// Unit Separator.
            /// </summary>
            /// <remarks>
            /// Decimal:31
            /// <br/>
            /// Hex:'\u001f'
            /// </remarks>
            public const char UnitSeparator = '\u001f';

            /// <summary>
            /// Space (" ").
            /// </summary>
            /// <remarks>
            /// Decimal:32
            /// <br/>
            /// Hex:'\u0020'
            /// </remarks>
            public const char Space = '\u0020';

            /// <summary>
            /// Exclamation Mark ("!").
            /// </summary>
            /// <remarks>
            /// Decimal:33
            /// <br/>
            /// Hex:'\u0021'
            /// </remarks>
            public const char ExclamationMark = '\u0021';

            /// <summary>
            /// Quotation Mark ("&quot;").
            /// </summary>
            /// <remarks>
            /// Decimal:34
            /// <br/>
            /// Hex:'\u0022'
            /// </remarks>
            public const char QuotationMark = '\u0022';

            /// <summary>
            /// Number Sign ("#").
            /// </summary>
            /// <remarks>
            /// Decimal:35
            /// <br/>
            /// Hex:'\u0023'
            /// </remarks>
            public const char NumberSign = '\u0023';

            /// <summary>
            /// Dollar Sign ("$").
            /// </summary>
            /// <remarks>
            /// Decimal:36
            /// <br/>
            /// Hex:'\u0024'
            /// </remarks>
            public const char DollarSign = '\u0024';

            /// <summary>
            /// Percent Sign ("%").
            /// </summary>
            /// <remarks>
            /// Decimal:37
            /// <br/>
            /// Hex:'\u0025'
            /// </remarks>
            public const char PercentSign = '\u0025';

            /// <summary>
            /// Ampersand ("&amp;").
            /// </summary>
            /// <remarks>
            /// Decimal:38
            /// <br/>
            /// Hex:'\u0026'
            /// </remarks>
            public const char Ampersand = '\u0026';

            /// <summary>
            /// Apostrophe ("&apos;").
            /// </summary>
            /// <remarks>
            /// Decimal:39
            /// <br/>
            /// Hex:'\u0027'
            /// </remarks>
            public const char Apostrophe = '\u0027';

            /// <summary>
            /// Left Parenthesis ("(").
            /// </summary>
            /// <remarks>
            /// Decimal:40
            /// <br/>
            /// Hex:'\u0028'
            /// </remarks>
            public const char LeftParenthesis = '\u0028';

            /// <summary>
            /// Right Parenthesis (")").
            /// </summary>
            /// <remarks>
            /// Decimal:41
            /// <br/>
            /// Hex:'\u0029'
            /// </remarks>
            public const char RightParenthesis = '\u0029';

            /// <summary>
            /// Asterisk ("*").
            /// </summary>
            /// <remarks>
            /// Decimal:42
            /// <br/>
            /// Hex:'\u002a'
            /// </remarks>
            public const char Asterisk = '\u002a';

            /// <summary>
            /// Plus Sign ("+").
            /// </summary>
            /// <remarks>
            /// Decimal:43
            /// <br/>
            /// Hex:'\u002b'
            /// </remarks>
            public const char PlusSign = '\u002b';

            /// <summary>
            /// Comma (",").
            /// </summary>
            /// <remarks>
            /// Decimal:44
            /// <br/>
            /// Hex:'\u002c'
            /// </remarks>
            public const char Comma = '\u002c';

            /// <summary>
            /// Hyphen-Minus ("-").
            /// </summary>
            /// <remarks>
            /// Decimal:45
            /// <br/>
            /// Hex:'\u002d'
            /// </remarks>
            public const char HyphenMinus = '\u002d';

            /// <summary>
            /// Full Stop (".").
            /// </summary>
            /// <remarks>
            /// Decimal:46
            /// <br/>
            /// Hex:'\u002e'
            /// </remarks>
            public const char FullStop = '\u002e';

            /// <summary>
            /// Solidus ("/").
            /// </summary>
            /// <remarks>
            /// Decimal:47
            /// <br/>
            /// Hex:'\u002f'
            /// </remarks>
            public const char Solidus = '\u002f';

            /// <summary>
            /// Digit Zero ("0").
            /// </summary>
            /// <remarks>
            /// Decimal:48
            /// <br/>
            /// Hex:'\u0030'
            /// </remarks>
            public const char DigitZero = '\u0030';

            /// <summary>
            /// Digit One ("1").
            /// </summary>
            /// <remarks>
            /// Decimal:49
            /// <br/>
            /// Hex:'\u0031'
            /// </remarks>
            public const char DigitOne = '\u0031';

            /// <summary>
            /// Digit Two ("2").
            /// </summary>
            /// <remarks>
            /// Decimal:50
            /// <br/>
            /// Hex:'\u0032'
            /// </remarks>
            public const char DigitTwo = '\u0032';

            /// <summary>
            /// Digit Three ("3").
            /// </summary>
            /// <remarks>
            /// Decimal:51
            /// <br/>
            /// Hex:'\u0033'
            /// </remarks>
            public const char DigitThree = '\u0033';

            /// <summary>
            /// Digit Four ("4").
            /// </summary>
            /// <remarks>
            /// Decimal:52
            /// <br/>
            /// Hex:'\u0034'
            /// </remarks>
            public const char DigitFour = '\u0034';

            /// <summary>
            /// Digit Five ("5").
            /// </summary>
            /// <remarks>
            /// Decimal:53
            /// <br/>
            /// Hex:'\u0035'
            /// </remarks>
            public const char DigitFive = '\u0035';

            /// <summary>
            /// Digit Six ("6").
            /// </summary>
            /// <remarks>
            /// Decimal:54
            /// <br/>
            /// Hex:'\u0036'
            /// </remarks>
            public const char DigitSix = '\u0036';

            /// <summary>
            /// Digit Seven ("7").
            /// </summary>
            /// <remarks>
            /// Decimal:55
            /// <br/>
            /// Hex:'\u0037'
            /// </remarks>
            public const char DigitSeven = '\u0037';

            /// <summary>
            /// Digit Eight ("8").
            /// </summary>
            /// <remarks>
            /// Decimal:56
            /// <br/>
            /// Hex:'\u0038'
            /// </remarks>
            public const char DigitEight = '\u0038';

            /// <summary>
            /// Digit Nine ("9").
            /// </summary>
            /// <remarks>
            /// Decimal:57
            /// <br/>
            /// Hex:'\u0039'
            /// </remarks>
            public const char DigitNine = '\u0039';

            /// <summary>
            /// Colon (":").
            /// </summary>
            /// <remarks>
            /// Decimal:58
            /// <br/>
            /// Hex:'\u003a'
            /// </remarks>
            public const char Colon = '\u003a';

            /// <summary>
            /// Semicolon (";").
            /// </summary>
            /// <remarks>
            /// Decimal:59
            /// <br/>
            /// Hex:'\u003b'
            /// </remarks>
            public const char Semicolon = '\u003b';

            /// <summary>
            /// Less-Than Sign ("&lt;").
            /// </summary>
            /// <remarks>
            /// Decimal:60
            /// <br/>
            /// Hex:'\u003c'
            /// </remarks>
            public const char LessThanSign = '\u003c';

            /// <summary>
            /// Equals Sign ("=").
            /// </summary>
            /// <remarks>
            /// Decimal:61
            /// <br/>
            /// Hex:'\u003d'
            /// </remarks>
            public const char EqualsSign = '\u003d';

            /// <summary>
            /// Greater-Than Sign ("&gt;").
            /// </summary>
            /// <remarks>
            /// Decimal:62
            /// <br/>
            /// Hex:'\u003e'
            /// </remarks>
            public const char GreaterThanSign = '\u003e';

            /// <summary>
            /// Question Mark ("?").
            /// </summary>
            /// <remarks>
            /// Decimal:63
            /// <br/>
            /// Hex:'\u003f'
            /// </remarks>
            public const char QuestionMark = '\u003f';

            /// <summary>
            /// Commercial At ("@").
            /// </summary>
            /// <remarks>
            /// Decimal:64
            /// <br/>
            /// Hex:'\u0040'
            /// </remarks>
            public const char CommercialAt = '\u0040';

            /// <summary>
            /// Latin Capital Letter A ("A").
            /// </summary>
            /// <remarks>
            /// Decimal:65
            /// <br/>
            /// Hex:'\u0041'
            /// </remarks>
            public const char LatinCapitalLetterA = '\u0041';

            /// <summary>
            /// Latin Capital Letter B ("B").
            /// </summary>
            /// <remarks>
            /// Decimal:66
            /// <br/>
            /// Hex:'\u0042'
            /// </remarks>
            public const char LatinCapitalLetterB = '\u0042';

            /// <summary>
            /// Latin Capital Letter C ("C").
            /// </summary>
            /// <remarks>
            /// Decimal:67
            /// <br/>
            /// Hex:'\u0043'
            /// </remarks>
            public const char LatinCapitalLetterC = '\u0043';

            /// <summary>
            /// Latin Capital Letter D ("D").
            /// </summary>
            /// <remarks>
            /// Decimal:68
            /// <br/>
            /// Hex:'\u0044'
            /// </remarks>
            public const char LatinCapitalLetterD = '\u0044';

            /// <summary>
            /// Latin Capital Letter E ("E").
            /// </summary>
            /// <remarks>
            /// Decimal:69
            /// <br/>
            /// Hex:'\u0045'
            /// </remarks>
            public const char LatinCapitalLetterE = '\u0045';

            /// <summary>
            /// Latin Capital Letter F ("F").
            /// </summary>
            /// <remarks>
            /// Decimal:70
            /// <br/>
            /// Hex:'\u0046'
            /// </remarks>
            public const char LatinCapitalLetterF = '\u0046';

            /// <summary>
            /// Latin Capital Letter G ("G").
            /// </summary>
            /// <remarks>
            /// Decimal:71
            /// <br/>
            /// Hex:'\u0047'
            /// </remarks>
            public const char LatinCapitalLetterG = '\u0047';

            /// <summary>
            /// Latin Capital Letter H ("H").
            /// </summary>
            /// <remarks>
            /// Decimal:72
            /// <br/>
            /// Hex:'\u0048'
            /// </remarks>
            public const char LatinCapitalLetterH = '\u0048';

            /// <summary>
            /// Latin Capital Letter I ("I").
            /// </summary>
            /// <remarks>
            /// Decimal:73
            /// <br/>
            /// Hex:'\u0049'
            /// </remarks>
            public const char LatinCapitalLetterI = '\u0049';

            /// <summary>
            /// Latin Capital Letter J ("J").
            /// </summary>
            /// <remarks>
            /// Decimal:74
            /// <br/>
            /// Hex:'\u004a'
            /// </remarks>
            public const char LatinCapitalLetterJ = '\u004a';

            /// <summary>
            /// Latin Capital Letter K ("K").
            /// </summary>
            /// <remarks>
            /// Decimal:75
            /// <br/>
            /// Hex:'\u004b'
            /// </remarks>
            public const char LatinCapitalLetterK = '\u004b';

            /// <summary>
            /// Latin Capital Letter L ("L").
            /// </summary>
            /// <remarks>
            /// Decimal:76
            /// <br/>
            /// Hex:'\u004c'
            /// </remarks>
            public const char LatinCapitalLetterL = '\u004c';

            /// <summary>
            /// Latin Capital Letter M ("M").
            /// </summary>
            /// <remarks>
            /// Decimal:77
            /// <br/>
            /// Hex:'\u004d'
            /// </remarks>
            public const char LatinCapitalLetterM = '\u004d';

            /// <summary>
            /// Latin Capital Letter N ("N").
            /// </summary>
            /// <remarks>
            /// Decimal:78
            /// <br/>
            /// Hex:'\u004e'
            /// </remarks>
            public const char LatinCapitalLetterN = '\u004e';

            /// <summary>
            /// Latin Capital Letter O ("O").
            /// </summary>
            /// <remarks>
            /// Decimal:79
            /// <br/>
            /// Hex:'\u004f'
            /// </remarks>
            public const char LatinCapitalLetterO = '\u004f';

            /// <summary>
            /// Latin Capital Letter P ("P").
            /// </summary>
            /// <remarks>
            /// Decimal:80
            /// <br/>
            /// Hex:'\u0050'
            /// </remarks>
            public const char LatinCapitalLetterP = '\u0050';

            /// <summary>
            /// Latin Capital Letter Q ("Q").
            /// </summary>
            /// <remarks>
            /// Decimal:81
            /// <br/>
            /// Hex:'\u0051'
            /// </remarks>
            public const char LatinCapitalLetterQ = '\u0051';

            /// <summary>
            /// Latin Capital Letter R ("R").
            /// </summary>
            /// <remarks>
            /// Decimal:82
            /// <br/>
            /// Hex:'\u0052'
            /// </remarks>
            public const char LatinCapitalLetterR = '\u0052';

            /// <summary>
            /// Latin Capital Letter S ("S").
            /// </summary>
            /// <remarks>
            /// Decimal:83
            /// <br/>
            /// Hex:'\u0053'
            /// </remarks>
            public const char LatinCapitalLetterS = '\u0053';

            /// <summary>
            /// Latin Capital Letter T ("T").
            /// </summary>
            /// <remarks>
            /// Decimal:84
            /// <br/>
            /// Hex:'\u0054'
            /// </remarks>
            public const char LatinCapitalLetterT = '\u0054';

            /// <summary>
            /// Latin Capital Letter U ("U").
            /// </summary>
            /// <remarks>
            /// Decimal:85
            /// <br/>
            /// Hex:'\u0055'
            /// </remarks>
            public const char LatinCapitalLetterU = '\u0055';

            /// <summary>
            /// Latin Capital Letter V ("V").
            /// </summary>
            /// <remarks>
            /// Decimal:86
            /// <br/>
            /// Hex:'\u0056'
            /// </remarks>
            public const char LatinCapitalLetterV = '\u0056';

            /// <summary>
            /// Latin Capital Letter W ("W").
            /// </summary>
            /// <remarks>
            /// Decimal:87
            /// <br/>
            /// Hex:'\u0057'
            /// </remarks>
            public const char LatinCapitalLetterW = '\u0057';

            /// <summary>
            /// Latin Capital Letter X ("X").
            /// </summary>
            /// <remarks>
            /// Decimal:88
            /// <br/>
            /// Hex:'\u0058'
            /// </remarks>
            public const char LatinCapitalLetterX = '\u0058';

            /// <summary>
            /// Latin Capital Letter Y ("Y").
            /// </summary>
            /// <remarks>
            /// Decimal:89
            /// <br/>
            /// Hex:'\u0059'
            /// </remarks>
            public const char LatinCapitalLetterY = '\u0059';

            /// <summary>
            /// Latin Capital Letter Z ("Z").
            /// </summary>
            /// <remarks>
            /// Decimal:90
            /// <br/>
            /// Hex:'\u005a'
            /// </remarks>
            public const char LatinCapitalLetterZ = '\u005a';

            /// <summary>
            /// Left Square Bracket ("[").
            /// </summary>
            /// <remarks>
            /// Decimal:91
            /// <br/>
            /// Hex:'\u005b'
            /// </remarks>
            public const char LeftSquareBracket = '\u005b';

            /// <summary>
            /// Reverse Solidus ("\").
            /// </summary>
            /// <remarks>
            /// Decimal:92
            /// <br/>
            /// Hex:'\u005c'
            /// </remarks>
            public const char ReverseSolidus = '\u005c';

            /// <summary>
            /// Right Square Bracket ("]").
            /// </summary>
            /// <remarks>
            /// Decimal:93
            /// <br/>
            /// Hex:'\u005d'
            /// </remarks>
            public const char RightSquareBracket = '\u005d';

            /// <summary>
            /// Circumflex Accent ("^").
            /// </summary>
            /// <remarks>
            /// Decimal:94
            /// <br/>
            /// Hex:'\u005e'
            /// </remarks>
            public const char CircumflexAccent = '\u005e';

            /// <summary>
            /// Low Line ("_").
            /// </summary>
            /// <remarks>
            /// Decimal:95
            /// <br/>
            /// Hex:'\u005f'
            /// </remarks>
            public const char LowLine = '\u005f';

            /// <summary>
            /// Grave Accent ("`").
            /// </summary>
            /// <remarks>
            /// Decimal:96
            /// <br/>
            /// Hex:'\u0060'
            /// </remarks>
            public const char GraveAccent = '\u0060';

            /// <summary>
            /// Latin Small Letter A ("a").
            /// </summary>
            /// <remarks>
            /// Decimal:97
            /// <br/>
            /// Hex:'\u0061'
            /// </remarks>
            public const char LatinSmallLetterA = '\u0061';

            /// <summary>
            /// Latin Small Letter B ("b").
            /// </summary>
            /// <remarks>
            /// Decimal:98
            /// <br/>
            /// Hex:'\u0062'
            /// </remarks>
            public const char LatinSmallLetterB = '\u0062';

            /// <summary>
            /// Latin Small Letter C ("c").
            /// </summary>
            /// <remarks>
            /// Decimal:99
            /// <br/>
            /// Hex:'\u0063'
            /// </remarks>
            public const char LatinSmallLetterC = '\u0063';

            /// <summary>
            /// Latin Small Letter D ("d").
            /// </summary>
            /// <remarks>
            /// Decimal:100
            /// <br/>
            /// Hex:'\u0064'
            /// </remarks>
            public const char LatinSmallLetterD = '\u0064';

            /// <summary>
            /// Latin Small Letter E ("e").
            /// </summary>
            /// <remarks>
            /// Decimal:101
            /// <br/>
            /// Hex:'\u0065'
            /// </remarks>
            public const char LatinSmallLetterE = '\u0065';

            /// <summary>
            /// Latin Small Letter F ("f").
            /// </summary>
            /// <remarks>
            /// Decimal:102
            /// <br/>
            /// Hex:'\u0066'
            /// </remarks>
            public const char LatinSmallLetterF = '\u0066';

            /// <summary>
            /// Latin Small Letter G ("g").
            /// </summary>
            /// <remarks>
            /// Decimal:103
            /// <br/>
            /// Hex:'\u0067'
            /// </remarks>
            public const char LatinSmallLetterG = '\u0067';

            /// <summary>
            /// Latin Small Letter H ("h").
            /// </summary>
            /// <remarks>
            /// Decimal:104
            /// <br/>
            /// Hex:'\u0068'
            /// </remarks>
            public const char LatinSmallLetterH = '\u0068';

            /// <summary>
            /// Latin Small Letter I ("i").
            /// </summary>
            /// <remarks>
            /// Decimal:105
            /// <br/>
            /// Hex:'\u0069'
            /// </remarks>
            public const char LatinSmallLetterI = '\u0069';

            /// <summary>
            /// Latin Small Letter J ("j").
            /// </summary>
            /// <remarks>
            /// Decimal:106
            /// <br/>
            /// Hex:'\u006a'
            /// </remarks>
            public const char LatinSmallLetterJ = '\u006a';

            /// <summary>
            /// Latin Small Letter K ("k").
            /// </summary>
            /// <remarks>
            /// Decimal:107
            /// <br/>
            /// Hex:'\u006b'
            /// </remarks>
            public const char LatinSmallLetterK = '\u006b';

            /// <summary>
            /// Latin Small Letter L ("l").
            /// </summary>
            /// <remarks>
            /// Decimal:108
            /// <br/>
            /// Hex:'\u006c'
            /// </remarks>
            public const char LatinSmallLetterL = '\u006c';

            /// <summary>
            /// Latin Small Letter M ("m").
            /// </summary>
            /// <remarks>
            /// Decimal:109
            /// <br/>
            /// Hex:'\u006d'
            /// </remarks>
            public const char LatinSmallLetterM = '\u006d';

            /// <summary>
            /// Latin Small Letter N ("n").
            /// </summary>
            /// <remarks>
            /// Decimal:110
            /// <br/>
            /// Hex:'\u006e'
            /// </remarks>
            public const char LatinSmallLetterN = '\u006e';

            /// <summary>
            /// Latin Small Letter O ("o").
            /// </summary>
            /// <remarks>
            /// Decimal:111
            /// <br/>
            /// Hex:'\u006f'
            /// </remarks>
            public const char LatinSmallLetterO = '\u006f';

            /// <summary>
            /// Latin Small Letter O ("o").
            /// </summary>
            /// <remarks>
            /// Decimal:112
            /// <br/>
            /// Hex:'\u0070'
            /// </remarks>
            public const char LatinSmallLetterP = '\u0070';

            /// <summary>
            /// Latin Small Letter Q ("q").
            /// </summary>
            /// <remarks>
            /// Decimal:113
            /// <br/>
            /// Hex:'\u0071'
            /// </remarks>
            public const char LatinSmallLetterQ = '\u0071';

            /// <summary>
            /// Latin Small Letter R ("r").
            /// </summary>
            /// <remarks>
            /// Decimal:114
            /// <br/>
            /// Hex:'\u0072'
            /// </remarks>
            public const char LatinSmallLetterR = '\u0072';

            /// <summary>
            /// Latin Small Letter S ("s").
            /// </summary>
            /// <remarks>
            /// Decimal:115
            /// <br/>
            /// Hex:'\u0073'
            /// </remarks>
            public const char LatinSmallLetterS = '\u0073';

            /// <summary>
            /// Latin Small Letter T ("t").
            /// </summary>
            /// <remarks>
            /// Decimal:116
            /// <br/>
            /// Hex:'\u0074'
            /// </remarks>
            public const char LatinSmallLetterT = '\u0074';

            /// <summary>
            /// Latin Small Letter U ("u").
            /// </summary>
            /// <remarks>
            /// Decimal:117
            /// <br/>
            /// Hex:'\u0075'
            /// </remarks>
            public const char LatinSmallLetterU = '\u0075';

            /// <summary>
            /// Latin Small Letter V ("v").
            /// </summary>
            /// <remarks>
            /// Decimal:118
            /// <br/>
            /// Hex:'\u0076'
            /// </remarks>
            public const char LatinSmallLetterV = '\u0076';

            /// <summary>
            /// Latin Small Letter W ("w").
            /// </summary>
            /// <remarks>
            /// Decimal:119
            /// <br/>
            /// Hex:'\u0077'
            /// </remarks>
            public const char LatinSmallLetterW = '\u0077';

            /// <summary>
            /// Latin Small Letter X ("x").
            /// </summary>
            /// <remarks>
            /// Decimal:120
            /// <br/>
            /// Hex:'\u0078'
            /// </remarks>
            public const char LatinSmallLetterX = '\u0078';

            /// <summary>
            /// Latin Small Letter Y ("y").
            /// </summary>
            /// <remarks>
            /// Decimal:121
            /// <br/>
            /// Hex:'\u0079'
            /// </remarks>
            public const char LatinSmallLetterY = '\u0079';

            /// <summary>
            /// Latin Small Letter Z ("z").
            /// </summary>
            /// <remarks>
            /// Decimal:122
            /// <br/>
            /// Hex:'\u007a'
            /// </remarks>
            public const char LatinSmallLetterZ = '\u007a';

            /// <summary>
            /// Left Curly Bracket ("{").
            /// </summary>
            /// <remarks>
            /// Decimal:123
            /// <br/>
            /// Hex:'\u007b'
            /// </remarks>
            public const char LeftCurlyBracket = '\u007b';

            /// <summary>
            /// Vertical Line ("|").
            /// </summary>
            /// <remarks>
            /// Decimal:124
            /// <br/>
            /// Hex:'\u007c'
            /// </remarks>
            public const char VerticalLine = '\u007c';

            /// <summary>
            /// Right Curly Bracket ("}").
            /// </summary>
            /// <remarks>
            /// Decimal:125
            /// <br/>
            /// Hex:'\u007d'
            /// </remarks>
            public const char RightCurlyBracket = '\u007d';

            /// <summary>
            /// Tilde ("~").
            /// </summary>
            /// <remarks>
            /// Decimal:126
            /// <br/>
            /// Hex:'\u007e'
            /// </remarks>
            public const char Tilde = '\u007e';

            /// <summary>
            /// Delete.
            /// </summary>
            /// <remarks>
            /// Decimal:127
            /// <br/>
            /// Hex:'\u007f'
            /// </remarks>
            public const char Delete = '\u007f';

            #endregion
        }
    }
}
