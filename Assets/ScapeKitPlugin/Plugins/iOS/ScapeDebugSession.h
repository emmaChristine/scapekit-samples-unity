
extern "C"
{
	void _setLogConfig(int level, int output);
	void _mockGPSCoordinates(double latitude, double longitude);
	void _log(int level, const char*_Nonnull tag, const char*_Nonnull message);
	void _saveImages(bool save);
}