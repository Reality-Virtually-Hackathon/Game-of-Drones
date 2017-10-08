#ifdef __cplusplus
extern "C" {
#else
    @interface MergeIOSNative: NSObject
#endif
bool HasCameraPermissions();
void RequestCameraPermission();
void OpenSettings ();
int HasPhotoPermission();
void RequestPhotoPermission();
void NativeOpenPhotoSettings();
#ifdef __cplusplus
}
#else
@end
#endif
