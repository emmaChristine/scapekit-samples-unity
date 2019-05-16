

extern "C"
{

double _metersBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2);

double _angleBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2);

void _wgsToLocal(double latitude, double longitude, double altitude, long cell_id, double* result);

void _localToWgs(double x, double y, double z, long cell_id, double* result);

long _cellIdForWgs(double latitude, double longitude);

}