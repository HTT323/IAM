namespace Iam.Common.Contracts
{
    public interface IBundle
    {
        string RenderCss(string html);
        string RenderJs(string html);
    }
}