/*
 * 工具类 (与游戏具体逻辑无关)
 */

public partial class GameUtils
{
    /// <summary>
    /// 是否为合法的字符串(表格中很多字符串配的是"-1");
    /// </summary>
    public static bool IsValidString(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        if (text == "-1")
            return false;

        return true;
    }
}