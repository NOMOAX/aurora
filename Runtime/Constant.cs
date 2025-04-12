using System.Text.RegularExpressions;

namespace Aurora
{
    /// <summary>
    /// 常量和静态只读量。
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// 字符串常量。
        /// </summary>
        public static class String
        {
            /// <summary>
            /// 作者的名称。
            /// </summary>
            public const string AuthorName = "谢凯文";

            /// <summary>
            /// 作者的英文名称。
            /// </summary>
            public const string AuthorNameEnglish = "Kevin Xie";

            /// <summary>
            /// 可使得单精度浮点数成功往返的格式。
            /// </summary>
            public const string FloatFormatRoundTrip = "G9";

            /// <summary>
            /// 可获取单精度浮点数有效定点数字的格式。
            /// </summary>
            public const string FloatFormatSignificantFixedPointFigures = "0.#########";

            /// <summary>
            /// 可使得双精度浮点数成功往返的格式。
            /// </summary>
            public const string DoubleFormatRoundTrip = "G17";

            /// <summary>
            /// 可获取双精度浮点数有效定点数字的格式。
            /// </summary>
            public const string DoubleFormatSignificantFixedPointFigures = "0.#################";

            /// <summary>
            /// 用于匹配符合 RFC 5322 标准的电子邮件地址的正则表达式模式。
            /// </summary>
            public const string EmailAddressRegexPattern =
                "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";
        }

        /// <summary>
        /// 正则表达式静态只读量。
        /// </summary>
        public static class Regex
        {
            /// <summary>
            /// 用于匹配符合 RFC 5322 标准的电子邮件地址的模式。
            /// </summary>
            public static readonly System.Text.RegularExpressions.Regex EmailAddressRegex =
                new System.Text.RegularExpressions.Regex(String.EmailAddressRegexPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// 时间间隔静态只读量。
        /// </summary>
        public static class TimeSpan
        {
            /// <summary>
            /// 计时器支持的最大超时时间间隔。
            /// </summary>
            public static readonly System.TimeSpan TimerMaxSupportedTimeout =
                System.TimeSpan.FromMilliseconds(4294967294);
        }

        /// <summary>
        /// 字符常量。
        /// </summary>
        public static class Character
        {
            #region 基本拉丁字母 ('\u0000' → '\u007f')

            /// <summary>
            /// 空。
            /// </summary>
            /// <remarks>
            /// 十进制：0
            /// <br/>
            /// 十六进制：'\u0000'
            /// <br/>
            /// 转义：'\0'
            /// </remarks>
            public const char Null = '\u0000';

            /// <summary>
            /// 标题开始。
            /// </summary>
            /// <remarks>
            /// 十进制：1
            /// <br/>
            /// 十六进制：'\u0001'
            /// </remarks>
            public const char StartOfHeading = '\u0001';

            /// <summary>
            /// 正文开始。
            /// </summary>
            /// <remarks>
            /// 十进制：2
            /// <br/>
            /// 十六进制：'\u0002'
            /// </remarks>
            public const char StartOfText = '\u0002';

            /// <summary>
            /// 正文结束。
            /// </summary>
            /// <remarks>
            /// 十进制：3
            /// <br/>
            /// 十六进制：'\u0003'
            /// </remarks>
            public const char EndOfText = '\u0003';

            /// <summary>
            /// 传输结束。
            /// </summary>
            /// <remarks>
            /// 十进制：4
            /// <br/>
            /// 十六进制：'\u0004'
            /// </remarks>
            public const char EndOfTransmission = '\u0004';

            /// <summary>
            /// 询问。
            /// </summary>
            /// <remarks>
            /// 十进制：5
            /// <br/>
            /// 十六进制：'\u0005'
            /// </remarks>
            public const char Enquiry = '\u0005';

            /// <summary>
            /// 确认。
            /// </summary>
            /// <remarks>
            /// 十进制：6
            /// <br/>
            /// 十六进制：'\u0006'
            /// </remarks>
            public const char Acknowledge = '\u0006';

            /// <summary>
            /// 响铃。
            /// </summary>
            /// <remarks>
            /// 十进制：7
            /// <br/>
            /// 十六进制：'\u0007'
            /// <br/>
            /// 转义：'\a'
            /// </remarks>
            public const char Bell = '\u0007';

            /// <summary>
            /// 退格。
            /// </summary>
            /// <remarks>
            /// 十进制：8
            /// <br/>
            /// 十六进制：'\u0008'
            /// <br/>
            /// 转义：'\b'
            /// </remarks>
            public const char Backspace = '\u0008';

            /// <summary>
            /// 水平制表符。
            /// </summary>
            /// <remarks>
            /// 十进制：9
            /// <br/>
            /// 十六进制：'\u0009'
            /// <br/>
            /// 转义：'\t'
            /// </remarks>
            public const char HorizontalTabulation = '\u0009';

            /// <summary>
            /// 换行。
            /// </summary>
            /// <remarks>
            /// 十进制：10
            /// <br/>
            /// 十六进制：'\u000a'
            /// <br/>
            /// 转义：'\n'
            /// </remarks>
            public const char NewLine = '\u000a';

            /// <summary>
            /// 垂直制表符。
            /// </summary>
            /// <remarks>
            /// 十进制：11
            /// <br/>
            /// 十六进制：'\u000b'
            /// <br/>
            /// 转义：'\v'
            /// </remarks>
            public const char VerticalTabulation = '\u000b';

            /// <summary>
            /// 换页。
            /// </summary>
            /// <remarks>
            /// 十进制：12
            /// <br/>
            /// 十六进制：'\u000c'
            /// <br/>
            /// 转义：'\f'
            /// </remarks>
            public const char FormFeed = '\u000c';

            /// <summary>
            /// 回车。
            /// </summary>
            /// <remarks>
            /// 十进制：13
            /// <br/>
            /// 十六进制：'\u000d'
            /// <br/>
            /// 转义：'\r'
            /// </remarks>
            public const char CarriageReturn = '\u000d';

            /// <summary>
            /// 移出。
            /// </summary>
            /// <remarks>
            /// 十进制：14
            /// <br/>
            /// 十六进制：'\u000e'
            /// </remarks>
            public const char ShiftOut = '\u000e';

            /// <summary>
            /// 移入。
            /// </summary>
            /// <remarks>
            /// 十进制：15
            /// <br/>
            /// 十六进制：'\u000f'
            /// </remarks>
            public const char ShiftIn = '\u000f';

            /// <summary>
            /// 数据传输转义。
            /// </summary>
            /// <remarks>
            /// 十进制：16
            /// <br/>
            /// 十六进制：'\u0010'
            /// </remarks>
            public const char DataLinkEscape = '\u0010';

            /// <summary>
            /// 设备控制一。
            /// </summary>
            /// <remarks>
            /// 十进制：17
            /// <br/>
            /// 十六进制：'\u0011'
            /// </remarks>
            public const char DeviceControlOne = '\u0011';

            /// <summary>
            /// 设备控制二。
            /// </summary>
            /// <remarks>
            /// 十进制：18
            /// <br/>
            /// 十六进制：'\u0012'
            /// </remarks>
            public const char DeviceControlTwo = '\u0012';

            /// <summary>
            /// 设备控制三。
            /// </summary>
            /// <remarks>
            /// 十进制：19
            /// <br/>
            /// 十六进制：'\u0013'
            /// </remarks>
            public const char DeviceControlThree = '\u0013';

            /// <summary>
            /// 设备控制四。
            /// </summary>
            /// <remarks>
            /// 十进制：20
            /// <br/>
            /// 十六进制：'\u0014'
            /// </remarks>
            public const char DeviceControlFour = '\u0014';

            /// <summary>
            /// 否认。
            /// </summary>
            /// <remarks>
            /// 十进制：21
            /// <br/>
            /// 十六进制：'\u0015'
            /// </remarks>
            public const char NegativeAcknowledge = '\u0015';

            /// <summary>
            /// 同步空闲。
            /// </summary>
            /// <remarks>
            /// 十进制：22
            /// <br/>
            /// 十六进制：'\u0016'
            /// </remarks>
            public const char SynchronousIdle = '\u0016';

            /// <summary>
            /// 传输块结束。
            /// </summary>
            /// <remarks>
            /// 十进制：23
            /// <br/>
            /// 十六进制：'\u0017'
            /// </remarks>
            public const char EndOfTransmissionBlock = '\u0017';

            /// <summary>
            /// 取消。
            /// </summary>
            /// <remarks>
            /// 十进制：24
            /// <br/>
            /// 十六进制：'\u0018'
            /// </remarks>
            public const char Cancel = '\u0018';

            /// <summary>
            /// 媒体结束。
            /// </summary>
            /// <remarks>
            /// 十进制：25
            /// <br/>
            /// 十六进制：'\u0019'
            /// </remarks>
            public const char EndOfMedium = '\u0019';

            /// <summary>
            /// 替换。
            /// </summary>
            /// <remarks>
            /// 十进制：26
            /// <br/>
            /// 十六进制：'\u001a'
            /// </remarks>
            public const char Substitute = '\u001a';

            /// <summary>
            /// 转义。
            /// </summary>
            /// <remarks>
            /// 十进制：27
            /// <br/>
            /// 十六进制：'\u001b'
            /// </remarks>
            public const char Escape = '\u001b';

            /// <summary>
            /// 文件分隔。
            /// </summary>
            /// <remarks>
            /// 十进制：28
            /// <br/>
            /// 十六进制：'\u001c'
            /// </remarks>
            public const char FileSeparator = '\u001c';

            /// <summary>
            /// 组分隔。
            /// </summary>
            /// <remarks>
            /// 十进制：29
            /// <br/>
            /// 十六进制：'\u001d'
            /// </remarks>
            public const char GroupSeparator = '\u001d';

            /// <summary>
            /// 记录分隔。
            /// </summary>
            /// <remarks>
            /// 十进制：30
            /// <br/>
            /// 十六进制：'\u001e'
            /// </remarks>
            public const char RecordSeparator = '\u001e';

            /// <summary>
            /// 单元分隔。
            /// </summary>
            /// <remarks>
            /// 十进制：31
            /// <br/>
            /// 十六进制：'\u001f'
            /// </remarks>
            public const char UnitSeparator = '\u001f';

            /// <summary>
            /// 空格（“ ”）。
            /// </summary>
            /// <remarks>
            /// 十进制：32
            /// <br/>
            /// 十六进制：'\u0020'
            /// </remarks>
            public const char Space = '\u0020';

            /// <summary>
            /// 叹号（“!”）。
            /// </summary>
            /// <remarks>
            /// 十进制：33
            /// <br/>
            /// 十六进制：'\u0021'
            /// </remarks>
            public const char ExclamationMark = '\u0021';

            /// <summary>
            /// 引号（“&quot;”）。
            /// </summary>
            /// <remarks>
            /// 十进制：34
            /// <br/>
            /// 十六进制：'\u0022'
            /// </remarks>
            public const char QuotationMark = '\u0022';

            /// <summary>
            /// 数字标记（“#”）。
            /// </summary>
            /// <remarks>
            /// 十进制：35
            /// <br/>
            /// 十六进制：'\u0023'
            /// </remarks>
            public const char NumberSign = '\u0023';

            /// <summary>
            /// 美元标记（“$”）。
            /// </summary>
            /// <remarks>
            /// 十进制：36
            /// <br/>
            /// 十六进制：'\u0024'
            /// </remarks>
            public const char DollarSign = '\u0024';

            /// <summary>
            /// 百分号（“%”）。
            /// </summary>
            /// <remarks>
            /// 十进制：37
            /// <br/>
            /// 十六进制：'\u0025'
            /// </remarks>
            public const char PercentSign = '\u0025';

            /// <summary>
            /// 与号（“&amp;”）。
            /// </summary>
            /// <remarks>
            /// 十进制：38
            /// <br/>
            /// 十六进制：'\u0026'
            /// </remarks>
            public const char Ampersand = '\u0026';

            /// <summary>
            /// 撇号（“&apos;”）。
            /// </summary>
            /// <remarks>
            /// 十进制：39
            /// <br/>
            /// 十六进制：'\u0027'
            /// </remarks>
            public const char Apostrophe = '\u0027';

            /// <summary>
            /// 左圆括号（“(”）。
            /// </summary>
            /// <remarks>
            /// 十进制：40
            /// <br/>
            /// 十六进制：'\u0028'
            /// </remarks>
            public const char LeftParenthesis = '\u0028';

            /// <summary>
            /// 右圆括号（“)”）。
            /// </summary>
            /// <remarks>
            /// 十进制：41
            /// <br/>
            /// 十六进制：'\u0029'
            /// </remarks>
            public const char RightParenthesis = '\u0029';

            /// <summary>
            /// 星号（“*”）。
            /// </summary>
            /// <remarks>
            /// 十进制：42
            /// <br/>
            /// 十六进制：'\u002a'
            /// </remarks>
            public const char Asterisk = '\u002a';

            /// <summary>
            /// 正号（“+”）。
            /// </summary>
            /// <remarks>
            /// 十进制：43
            /// <br/>
            /// 十六进制：'\u002b'
            /// </remarks>
            public const char PlusSign = '\u002b';

            /// <summary>
            /// 逗号（“,”）。
            /// </summary>
            /// <remarks>
            /// 十进制：44
            /// <br/>
            /// 十六进制：'\u002c'
            /// </remarks>
            public const char Comma = '\u002c';

            /// <summary>
            /// 连字符及负号（“-”）。
            /// </summary>
            /// <remarks>
            /// 十进制：45
            /// <br/>
            /// 十六进制：'\u002d'
            /// </remarks>
            public const char HyphenMinus = '\u002d';

            /// <summary>
            /// 句号（“.”）。
            /// </summary>
            /// <remarks>
            /// 十进制：46
            /// <br/>
            /// 十六进制：'\u002e'
            /// </remarks>
            public const char FullStop = '\u002e';

            /// <summary>
            /// 斜线号（“/”）。
            /// </summary>
            /// <remarks>
            /// 十进制：47
            /// <br/>
            /// 十六进制：'\u002f'
            /// </remarks>
            public const char Solidus = '\u002f';

            /// <summary>
            /// 数字零（“0”）。
            /// </summary>
            /// <remarks>
            /// 十进制：48
            /// <br/>
            /// 十六进制：'\u0030'
            /// </remarks>
            public const char DigitZero = '\u0030';

            /// <summary>
            /// 数字一（“1”）。
            /// </summary>
            /// <remarks>
            /// 十进制：49
            /// <br/>
            /// 十六进制：'\u0031'
            /// </remarks>
            public const char DigitOne = '\u0031';

            /// <summary>
            /// 数字二（“2”）。
            /// </summary>
            /// <remarks>
            /// 十进制：50
            /// <br/>
            /// 十六进制：'\u0032'
            /// </remarks>
            public const char DigitTwo = '\u0032';

            /// <summary>
            /// 数字三（“3”）。
            /// </summary>
            /// <remarks>
            /// 十进制：51
            /// <br/>
            /// 十六进制：'\u0033'
            /// </remarks>
            public const char DigitThree = '\u0033';

            /// <summary>
            /// 数字四（“4”）。
            /// </summary>
            /// <remarks>
            /// 十进制：52
            /// <br/>
            /// 十六进制：'\u0034'
            /// </remarks>
            public const char DigitFour = '\u0034';

            /// <summary>
            /// 数字五（“5”）。
            /// </summary>
            /// <remarks>
            /// 十进制：53
            /// <br/>
            /// 十六进制：'\u0035'
            /// </remarks>
            public const char DigitFive = '\u0035';

            /// <summary>
            /// 数字六（“6”）。
            /// </summary>
            /// <remarks>
            /// 十进制：54
            /// <br/>
            /// 十六进制：'\u0036'
            /// </remarks>
            public const char DigitSix = '\u0036';

            /// <summary>
            /// 数字七（“7”）。
            /// </summary>
            /// <remarks>
            /// 十进制：55
            /// <br/>
            /// 十六进制：'\u0037'
            /// </remarks>
            public const char DigitSeven = '\u0037';

            /// <summary>
            /// 数字八（“8”）。
            /// </summary>
            /// <remarks>
            /// 十进制：56
            /// <br/>
            /// 十六进制：'\u0038'
            /// </remarks>
            public const char DigitEight = '\u0038';

            /// <summary>
            /// 数字九（“9”）。
            /// </summary>
            /// <remarks>
            /// 十进制：57
            /// <br/>
            /// 十六进制：'\u0039'
            /// </remarks>
            public const char DigitNine = '\u0039';

            /// <summary>
            /// 冒号（“:”）。
            /// </summary>
            /// <remarks>
            /// 十进制：58
            /// <br/>
            /// 十六进制：'\u003a'
            /// </remarks>
            public const char Colon = '\u003a';

            /// <summary>
            /// 分号（“;”）。
            /// </summary>
            /// <remarks>
            /// 十进制：59
            /// <br/>
            /// 十六进制：'\u003b'
            /// </remarks>
            public const char Semicolon = '\u003b';

            /// <summary>
            /// 小于号（“&lt;”）。
            /// </summary>
            /// <remarks>
            /// 十进制：60
            /// <br/>
            /// 十六进制：'\u003c'
            /// </remarks>
            public const char LessThanSign = '\u003c';

            /// <summary>
            /// 等号（“=”）。
            /// </summary>
            /// <remarks>
            /// 十进制：61
            /// <br/>
            /// 十六进制：'\u003d'
            /// </remarks>
            public const char EqualsSign = '\u003d';

            /// <summary>
            /// 大于号（“&gt;”）。
            /// </summary>
            /// <remarks>
            /// 十进制：62
            /// <br/>
            /// 十六进制：'\u003e'
            /// </remarks>
            public const char GreaterThanSign = '\u003e';

            /// <summary>
            /// 问号（“?”）。
            /// </summary>
            /// <remarks>
            /// 十进制：63
            /// <br/>
            /// 十六进制：'\u003f'
            /// </remarks>
            public const char QuestionMark = '\u003f';

            /// <summary>
            /// 商业用 At 符号（“@”）。
            /// </summary>
            /// <remarks>
            /// 十进制：64
            /// <br/>
            /// 十六进制：'\u0040'
            /// </remarks>
            public const char CommercialAt = '\u0040';

            /// <summary>
            /// 拉丁文大写字母 A（“A”）。
            /// </summary>
            /// <remarks>
            /// 十进制：65
            /// <br/>
            /// 十六进制：'\u0041'
            /// </remarks>
            public const char LatinCapitalLetterA = '\u0041';

            /// <summary>
            /// 拉丁文大写字母 B（“B”）。
            /// </summary>
            /// <remarks>
            /// 十进制：66
            /// <br/>
            /// 十六进制：'\u0042'
            /// </remarks>
            public const char LatinCapitalLetterB = '\u0042';

            /// <summary>
            /// 拉丁文大写字母 C（“C”）。
            /// </summary>
            /// <remarks>
            /// 十进制：67
            /// <br/>
            /// 十六进制：'\u0043'
            /// </remarks>
            public const char LatinCapitalLetterC = '\u0043';

            /// <summary>
            /// 拉丁文大写字母 D（“D”）。
            /// </summary>
            /// <remarks>
            /// 十进制：68
            /// <br/>
            /// 十六进制：'\u0044'
            /// </remarks>
            public const char LatinCapitalLetterD = '\u0044';

            /// <summary>
            /// 拉丁文大写字母 E（“E”）。
            /// </summary>
            /// <remarks>
            /// 十进制：69
            /// <br/>
            /// 十六进制：'\u0045'
            /// </remarks>
            public const char LatinCapitalLetterE = '\u0045';

            /// <summary>
            /// 拉丁文大写字母 F（“F”）。
            /// </summary>
            /// <remarks>
            /// 十进制：70
            /// <br/>
            /// 十六进制：'\u0046'
            /// </remarks>
            public const char LatinCapitalLetterF = '\u0046';

            /// <summary>
            /// 拉丁文大写字母 G（“G”）。
            /// </summary>
            /// <remarks>
            /// 十进制：71
            /// <br/>
            /// 十六进制：'\u0047'
            /// </remarks>
            public const char LatinCapitalLetterG = '\u0047';

            /// <summary>
            /// 拉丁文大写字母 H（“H”）。
            /// </summary>
            /// <remarks>
            /// 十进制：72
            /// <br/>
            /// 十六进制：'\u0048'
            /// </remarks>
            public const char LatinCapitalLetterH = '\u0048';

            /// <summary>
            /// 拉丁文大写字母 I（“I”）。
            /// </summary>
            /// <remarks>
            /// 十进制：73
            /// <br/>
            /// 十六进制：'\u0049'
            /// </remarks>
            public const char LatinCapitalLetterI = '\u0049';

            /// <summary>
            /// 拉丁文大写字母 J（“J”）。
            /// </summary>
            /// <remarks>
            /// 十进制：74
            /// <br/>
            /// 十六进制：'\u004a'
            /// </remarks>
            public const char LatinCapitalLetterJ = '\u004a';

            /// <summary>
            /// 拉丁文大写字母 K（“K”）。
            /// </summary>
            /// <remarks>
            /// 十进制：75
            /// <br/>
            /// 十六进制：'\u004b'
            /// </remarks>
            public const char LatinCapitalLetterK = '\u004b';

            /// <summary>
            /// 拉丁文大写字母 L（“L”）。
            /// </summary>
            /// <remarks>
            /// 十进制：76
            /// <br/>
            /// 十六进制：'\u004c'
            /// </remarks>
            public const char LatinCapitalLetterL = '\u004c';

            /// <summary>
            /// 拉丁文大写字母 M（“M”）。
            /// </summary>
            /// <remarks>
            /// 十进制：77
            /// <br/>
            /// 十六进制：'\u004d'
            /// </remarks>
            public const char LatinCapitalLetterM = '\u004d';

            /// <summary>
            /// 拉丁文大写字母 N（“N”）。
            /// </summary>
            /// <remarks>
            /// 十进制：78
            /// <br/>
            /// 十六进制：'\u004e'
            /// </remarks>
            public const char LatinCapitalLetterN = '\u004e';

            /// <summary>
            /// 拉丁文大写字母 O（“O”）。
            /// </summary>
            /// <remarks>
            /// 十进制：79
            /// <br/>
            /// 十六进制：'\u004f'
            /// </remarks>
            public const char LatinCapitalLetterO = '\u004f';

            /// <summary>
            /// 拉丁文大写字母 P（“P”）。
            /// </summary>
            /// <remarks>
            /// 十进制：80
            /// <br/>
            /// 十六进制：'\u0050'
            /// </remarks>
            public const char LatinCapitalLetterP = '\u0050';

            /// <summary>
            /// 拉丁文大写字母 Q（“Q”）。
            /// </summary>
            /// <remarks>
            /// 十进制：81
            /// <br/>
            /// 十六进制：'\u0051'
            /// </remarks>
            public const char LatinCapitalLetterQ = '\u0051';

            /// <summary>
            /// 拉丁文大写字母 R（“R”）。
            /// </summary>
            /// <remarks>
            /// 十进制：82
            /// <br/>
            /// 十六进制：'\u0052'
            /// </remarks>
            public const char LatinCapitalLetterR = '\u0052';

            /// <summary>
            /// 拉丁文大写字母 S（“S”）。
            /// </summary>
            /// <remarks>
            /// 十进制：83
            /// <br/>
            /// 十六进制：'\u0053'
            /// </remarks>
            public const char LatinCapitalLetterS = '\u0053';

            /// <summary>
            /// 拉丁文大写字母 T（“T”）。
            /// </summary>
            /// <remarks>
            /// 十进制：84
            /// <br/>
            /// 十六进制：'\u0054'
            /// </remarks>
            public const char LatinCapitalLetterT = '\u0054';

            /// <summary>
            /// 拉丁文大写字母 U（“U”）。
            /// </summary>
            /// <remarks>
            /// 十进制：85
            /// <br/>
            /// 十六进制：'\u0055'
            /// </remarks>
            public const char LatinCapitalLetterU = '\u0055';

            /// <summary>
            /// 拉丁文大写字母 V（“V”）。
            /// </summary>
            /// <remarks>
            /// 十进制：86
            /// <br/>
            /// 十六进制：'\u0056'
            /// </remarks>
            public const char LatinCapitalLetterV = '\u0056';

            /// <summary>
            /// 拉丁文大写字母 W（“W”）。
            /// </summary>
            /// <remarks>
            /// 十进制：87
            /// <br/>
            /// 十六进制：'\u0057'
            /// </remarks>
            public const char LatinCapitalLetterW = '\u0057';

            /// <summary>
            /// 拉丁文大写字母 X（“X”）。
            /// </summary>
            /// <remarks>
            /// 十进制：88
            /// <br/>
            /// 十六进制：'\u0058'
            /// </remarks>
            public const char LatinCapitalLetterX = '\u0058';

            /// <summary>
            /// 拉丁文大写字母 Y（“Y”）。
            /// </summary>
            /// <remarks>
            /// 十进制：89
            /// <br/>
            /// 十六进制：'\u0059'
            /// </remarks>
            public const char LatinCapitalLetterY = '\u0059';

            /// <summary>
            /// 拉丁文大写字母 Z（“Z”）。
            /// </summary>
            /// <remarks>
            /// 十进制：90
            /// <br/>
            /// 十六进制：'\u005a'
            /// </remarks>
            public const char LatinCapitalLetterZ = '\u005a';

            /// <summary>
            /// 左方括号（“[”）。
            /// </summary>
            /// <remarks>
            /// 十进制：91
            /// <br/>
            /// 十六进制：'\u005b'
            /// </remarks>
            public const char LeftSquareBracket = '\u005b';

            /// <summary>
            /// 反斜线号（“\”）。
            /// </summary>
            /// <remarks>
            /// 十进制：92
            /// <br/>
            /// 十六进制：'\u005c'
            /// </remarks>
            public const char ReverseSolidus = '\u005c';

            /// <summary>
            /// 右方括号（“]”）。
            /// </summary>
            /// <remarks>
            /// 十进制：93
            /// <br/>
            /// 十六进制：'\u005d'
            /// </remarks>
            public const char RightSquareBracket = '\u005d';

            /// <summary>
            /// 扬抑符（“^”）。
            /// </summary>
            /// <remarks>
            /// 十进制：94
            /// <br/>
            /// 十六进制：'\u005e'
            /// </remarks>
            public const char CircumflexAccent = '\u005e';

            /// <summary>
            /// 下横线（“_”）。
            /// </summary>
            /// <remarks>
            /// 十进制：95
            /// <br/>
            /// 十六进制：'\u005f'
            /// </remarks>
            public const char LowLine = '\u005f';

            /// <summary>
            /// 抑音符（“`”）。
            /// </summary>
            /// <remarks>
            /// 十进制：96
            /// <br/>
            /// 十六进制：'\u0060'
            /// </remarks>
            public const char GraveAccent = '\u0060';

            /// <summary>
            /// 拉丁文小写字母 A（“a”）。
            /// </summary>
            /// <remarks>
            /// 十进制：97
            /// <br/>
            /// 十六进制：'\u0061'
            /// </remarks>
            public const char LatinSmallLetterA = '\u0061';

            /// <summary>
            /// 拉丁文小写字母 B（“b”）。
            /// </summary>
            /// <remarks>
            /// 十进制：98
            /// <br/>
            /// 十六进制：'\u0062'
            /// </remarks>
            public const char LatinSmallLetterB = '\u0062';

            /// <summary>
            /// 拉丁文小写字母 C（“c”）。
            /// </summary>
            /// <remarks>
            /// 十进制：99
            /// <br/>
            /// 十六进制：'\u0063'
            /// </remarks>
            public const char LatinSmallLetterC = '\u0063';

            /// <summary>
            /// 拉丁文小写字母 D（“d”）。
            /// </summary>
            /// <remarks>
            /// 十进制：100
            /// <br/>
            /// 十六进制：'\u0064'
            /// </remarks>
            public const char LatinSmallLetterD = '\u0064';

            /// <summary>
            /// 拉丁文小写字母 E（“e”）。
            /// </summary>
            /// <remarks>
            /// 十进制：101
            /// <br/>
            /// 十六进制：'\u0065'
            /// </remarks>
            public const char LatinSmallLetterE = '\u0065';

            /// <summary>
            /// 拉丁文小写字母 F（“f”）。
            /// </summary>
            /// <remarks>
            /// 十进制：102
            /// <br/>
            /// 十六进制：'\u0066'
            /// </remarks>
            public const char LatinSmallLetterF = '\u0066';

            /// <summary>
            /// 拉丁文小写字母 G（“g”）。
            /// </summary>
            /// <remarks>
            /// 十进制：103
            /// <br/>
            /// 十六进制：'\u0067'
            /// </remarks>
            public const char LatinSmallLetterG = '\u0067';

            /// <summary>
            /// 拉丁文小写字母 H（“h”）。
            /// </summary>
            /// <remarks>
            /// 十进制：104
            /// <br/>
            /// 十六进制：'\u0068'
            /// </remarks>
            public const char LatinSmallLetterH = '\u0068';

            /// <summary>
            /// 拉丁文小写字母 I（“i”）。
            /// </summary>
            /// <remarks>
            /// 十进制：105
            /// <br/>
            /// 十六进制：'\u0069'
            /// </remarks>
            public const char LatinSmallLetterI = '\u0069';

            /// <summary>
            /// 拉丁文小写字母 J（“j”）。
            /// </summary>
            /// <remarks>
            /// 十进制：106
            /// <br/>
            /// 十六进制：'\u006a'
            /// </remarks>
            public const char LatinSmallLetterJ = '\u006a';

            /// <summary>
            /// 拉丁文小写字母 K（“k”）。
            /// </summary>
            /// <remarks>
            /// 十进制：107
            /// <br/>
            /// 十六进制：'\u006b'
            /// </remarks>
            public const char LatinSmallLetterK = '\u006b';

            /// <summary>
            /// 拉丁文小写字母 L（“l”）。
            /// </summary>
            /// <remarks>
            /// 十进制：108
            /// <br/>
            /// 十六进制：'\u006c'
            /// </remarks>
            public const char LatinSmallLetterL = '\u006c';

            /// <summary>
            /// 拉丁文小写字母 M（“m”）。
            /// </summary>
            /// <remarks>
            /// 十进制：109
            /// <br/>
            /// 十六进制：'\u006d'
            /// </remarks>
            public const char LatinSmallLetterM = '\u006d';

            /// <summary>
            /// 拉丁文小写字母 N（“n”）。
            /// </summary>
            /// <remarks>
            /// 十进制：110
            /// <br/>
            /// 十六进制：'\u006e'
            /// </remarks>
            public const char LatinSmallLetterN = '\u006e';

            /// <summary>
            /// 拉丁文小写字母 O（“o”）。
            /// </summary>
            /// <remarks>
            /// 十进制：111
            /// <br/>
            /// 十六进制：'\u006f'
            /// </remarks>
            public const char LatinSmallLetterO = '\u006f';

            /// <summary>
            /// 拉丁文小写字母 O（“o”）。
            /// </summary>
            /// <remarks>
            /// 十进制：112
            /// <br/>
            /// 十六进制：'\u0070'
            /// </remarks>
            public const char LatinSmallLetterP = '\u0070';

            /// <summary>
            /// 拉丁文小写字母 Q（“q”）。
            /// </summary>
            /// <remarks>
            /// 十进制：113
            /// <br/>
            /// 十六进制：'\u0071'
            /// </remarks>
            public const char LatinSmallLetterQ = '\u0071';

            /// <summary>
            /// 拉丁文小写字母 R（“r”）。
            /// </summary>
            /// <remarks>
            /// 十进制：114
            /// <br/>
            /// 十六进制：'\u0072'
            /// </remarks>
            public const char LatinSmallLetterR = '\u0072';

            /// <summary>
            /// 拉丁文小写字母 S（“s”）。
            /// </summary>
            /// <remarks>
            /// 十进制：115
            /// <br/>
            /// 十六进制：'\u0073'
            /// </remarks>
            public const char LatinSmallLetterS = '\u0073';

            /// <summary>
            /// 拉丁文小写字母 T（“t”）。
            /// </summary>
            /// <remarks>
            /// 十进制：116
            /// <br/>
            /// 十六进制：'\u0074'
            /// </remarks>
            public const char LatinSmallLetterT = '\u0074';

            /// <summary>
            /// 拉丁文小写字母 U（“u”）。
            /// </summary>
            /// <remarks>
            /// 十进制：117
            /// <br/>
            /// 十六进制：'\u0075'
            /// </remarks>
            public const char LatinSmallLetterU = '\u0075';

            /// <summary>
            /// 拉丁文小写字母 V（“v”）。
            /// </summary>
            /// <remarks>
            /// 十进制：118
            /// <br/>
            /// 十六进制：'\u0076'
            /// </remarks>
            public const char LatinSmallLetterV = '\u0076';

            /// <summary>
            /// 拉丁文小写字母 W（“w”）。
            /// </summary>
            /// <remarks>
            /// 十进制：119
            /// <br/>
            /// 十六进制：'\u0077'
            /// </remarks>
            public const char LatinSmallLetterW = '\u0077';

            /// <summary>
            /// 拉丁文小写字母 X（“x”）。
            /// </summary>
            /// <remarks>
            /// 十进制：120
            /// <br/>
            /// 十六进制：'\u0078'
            /// </remarks>
            public const char LatinSmallLetterX = '\u0078';

            /// <summary>
            /// 拉丁文小写字母 Y（“y”）。
            /// </summary>
            /// <remarks>
            /// 十进制：121
            /// <br/>
            /// 十六进制：'\u0079'
            /// </remarks>
            public const char LatinSmallLetterY = '\u0079';

            /// <summary>
            /// 拉丁文小写字母 Z（“z”）。
            /// </summary>
            /// <remarks>
            /// 十进制：122
            /// <br/>
            /// 十六进制：'\u007a'
            /// </remarks>
            public const char LatinSmallLetterZ = '\u007a';

            /// <summary>
            /// 左花括号（“{”）。
            /// </summary>
            /// <remarks>
            /// 十进制：123
            /// <br/>
            /// 十六进制：'\u007b'
            /// </remarks>
            public const char LeftCurlyBracket = '\u007b';

            /// <summary>
            /// 竖线（“|”）。
            /// </summary>
            /// <remarks>
            /// 十进制：124
            /// <br/>
            /// 十六进制：'\u007c'
            /// </remarks>
            public const char VerticalLine = '\u007c';

            /// <summary>
            /// 右花括号（“}”）。
            /// </summary>
            /// <remarks>
            /// 十进制：125
            /// <br/>
            /// 十六进制：'\u007d'
            /// </remarks>
            public const char RightCurlyBracket = '\u007d';

            /// <summary>
            /// 鄂化符及波浪号（“~”）。
            /// </summary>
            /// <remarks>
            /// 十进制：126
            /// <br/>
            /// 十六进制：'\u007e'
            /// </remarks>
            public const char Tilde = '\u007e';

            /// <summary>
            /// 删除。
            /// </summary>
            /// <remarks>
            /// 十进制：127
            /// <br/>
            /// 十六进制：'\u007f'
            /// </remarks>
            public const char Delete = '\u007f';

            #endregion
        }
    }
}
