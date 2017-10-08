
#import <Social/Social.h>
#import <Foundation/NSException.h>
#import <AVFoundation/AVFoundation.h>
#import <UIKit/UIKit.h>
#import <Photos/Photos.h>

bool HasCameraPermissions()
{
    AVAuthorizationStatus authStatus = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
    if(authStatus == AVAuthorizationStatusAuthorized)
        return true;
    else
        return false;
}
void RequestCameraPermission()
{
    if ([AVCaptureDevice respondsToSelector:@selector(requestAccessForMediaType:completionHandler:)]) {
        [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {}];
    }
}

void OpenSettings () {
    NSURL * url = [NSURL URLWithString: UIApplicationOpenSettingsURLString];
    [[UIApplication sharedApplication] openURL: url];
}

int HasPhotoPermission()
{
    int photoCode = 0;
    PHAuthorizationStatus status = [PHPhotoLibrary authorizationStatus];
    if (status == PHAuthorizationStatusAuthorized) {
        photoCode = 1;
    }
    else if (status == PHAuthorizationStatusDenied) {
        photoCode = 0;
    }
    else if (status == PHAuthorizationStatusNotDetermined) {
        photoCode = 2;
    }
    else if (status == PHAuthorizationStatusRestricted) {
        photoCode = 3;
    }
    return photoCode;
}
void RequestPhotoPermission(){
        dispatch_async(dispatch_get_main_queue(), ^{
            [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status) {}];
        });
}
void NativeOpenPhotoSettings(){
    NSString *accessDescription = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"NSPhotoLibraryUsageDescription"];
    UIAlertController * alertController = [UIAlertController alertControllerWithTitle:accessDescription message:@"To give permissions tap on 'Change Settings' button" preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleCancel handler:nil];
    [alertController addAction:cancelAction];
    
    UIAlertAction *settingsAction = [UIAlertAction actionWithTitle:@"Change Settings" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
    }];
    [alertController addAction:settingsAction];
    
    [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:alertController animated:YES completion:nil];
}

