
extern "C"
{
	void _setLogConfig(int level, int output);
	void _log(int level, const char*_Nonnull tag, const char*_Nonnull message);
}