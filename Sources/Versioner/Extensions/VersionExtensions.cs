namespace Versioner.Extensions
{
    public static class VersionExtensions
    {
        public static string ToTouchShortVersion(this Version version)
        {
            return string.Format("{0}.{1}.{2}", version.A, version.B, version.C);
        }

        public static string ToTouchBundleVersion(this Version version)
        {
            return string.Format("{0}", version.D);
        }
    }
}