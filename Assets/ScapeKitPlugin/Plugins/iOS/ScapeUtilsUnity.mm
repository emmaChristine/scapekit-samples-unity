

#import <ScapeKit/SCKScapeUtils.h>


extern "C"
{

double _metersBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2)
{
    double m = [SCKScapeUtils metersBetweenCoordinates:latitude1 longitude1:longitude1 latitude2:latitude2 longitude2:longitude2];
    return m;
}

double _angleBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2)
{
    double a = [SCKScapeUtils angleBetweenCoordinates:latitude1 longitude1:longitude1 latitude2:latitude2 longitude2:longitude2];
    return a;
}

void _wgsToLocal(double latitude, double longitude, double altitude, long cell_id, double* result)
{
    NSArray<NSNumber *> * res = [SCKScapeUtils wgsToLocal:latitude longitude:longitude altitude:altitude cellId:cell_id];
    
    for (int i = 0; i < 3; ++i) {
        result[i] = [res[i] doubleValue];
    }
}
void _localToWgs(double x, double y, double z, long cell_id, double* result)
{
    NSArray<NSNumber *> * res = [SCKScapeUtils localToWgs:x y:y z:z cellId:cell_id];
    for (int i = 0; i < 3; ++i) {
      result[i] = [res[i] doubleValue];
    }
}

long _cellIdForWgs(double latitude, double longitude)
{
    long cellId = [SCKScapeUtils cellIdForWgs:latitude longitude:longitude];
    return cellId;
}

}