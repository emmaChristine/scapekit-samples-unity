// Generated by Apple Swift version 5.0.1 (swiftlang-1001.0.82.4 clang-1001.0.46.5)
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wgcc-compat"

#if !defined(__has_include)
# define __has_include(x) 0
#endif
#if !defined(__has_attribute)
# define __has_attribute(x) 0
#endif
#if !defined(__has_feature)
# define __has_feature(x) 0
#endif
#if !defined(__has_warning)
# define __has_warning(x) 0
#endif

#if __has_include(<swift/objc-prologue.h>)
# include <swift/objc-prologue.h>
#endif

#pragma clang diagnostic ignored "-Wauto-import"
#include <Foundation/Foundation.h>
#include <stdint.h>
#include <stddef.h>
#include <stdbool.h>

#if !defined(SWIFT_TYPEDEFS)
# define SWIFT_TYPEDEFS 1
# if __has_include(<uchar.h>)
#  include <uchar.h>
# elif !defined(__cplusplus)
typedef uint_least16_t char16_t;
typedef uint_least32_t char32_t;
# endif
typedef float swift_float2  __attribute__((__ext_vector_type__(2)));
typedef float swift_float3  __attribute__((__ext_vector_type__(3)));
typedef float swift_float4  __attribute__((__ext_vector_type__(4)));
typedef double swift_double2  __attribute__((__ext_vector_type__(2)));
typedef double swift_double3  __attribute__((__ext_vector_type__(3)));
typedef double swift_double4  __attribute__((__ext_vector_type__(4)));
typedef int swift_int2  __attribute__((__ext_vector_type__(2)));
typedef int swift_int3  __attribute__((__ext_vector_type__(3)));
typedef int swift_int4  __attribute__((__ext_vector_type__(4)));
typedef unsigned int swift_uint2  __attribute__((__ext_vector_type__(2)));
typedef unsigned int swift_uint3  __attribute__((__ext_vector_type__(3)));
typedef unsigned int swift_uint4  __attribute__((__ext_vector_type__(4)));
#endif

#if !defined(SWIFT_PASTE)
# define SWIFT_PASTE_HELPER(x, y) x##y
# define SWIFT_PASTE(x, y) SWIFT_PASTE_HELPER(x, y)
#endif
#if !defined(SWIFT_METATYPE)
# define SWIFT_METATYPE(X) Class
#endif
#if !defined(SWIFT_CLASS_PROPERTY)
# if __has_feature(objc_class_property)
#  define SWIFT_CLASS_PROPERTY(...) __VA_ARGS__
# else
#  define SWIFT_CLASS_PROPERTY(...)
# endif
#endif

#if __has_attribute(objc_runtime_name)
# define SWIFT_RUNTIME_NAME(X) __attribute__((objc_runtime_name(X)))
#else
# define SWIFT_RUNTIME_NAME(X)
#endif
#if __has_attribute(swift_name)
# define SWIFT_COMPILE_NAME(X) __attribute__((swift_name(X)))
#else
# define SWIFT_COMPILE_NAME(X)
#endif
#if __has_attribute(objc_method_family)
# define SWIFT_METHOD_FAMILY(X) __attribute__((objc_method_family(X)))
#else
# define SWIFT_METHOD_FAMILY(X)
#endif
#if __has_attribute(noescape)
# define SWIFT_NOESCAPE __attribute__((noescape))
#else
# define SWIFT_NOESCAPE
#endif
#if __has_attribute(warn_unused_result)
# define SWIFT_WARN_UNUSED_RESULT __attribute__((warn_unused_result))
#else
# define SWIFT_WARN_UNUSED_RESULT
#endif
#if __has_attribute(noreturn)
# define SWIFT_NORETURN __attribute__((noreturn))
#else
# define SWIFT_NORETURN
#endif
#if !defined(SWIFT_CLASS_EXTRA)
# define SWIFT_CLASS_EXTRA
#endif
#if !defined(SWIFT_PROTOCOL_EXTRA)
# define SWIFT_PROTOCOL_EXTRA
#endif
#if !defined(SWIFT_ENUM_EXTRA)
# define SWIFT_ENUM_EXTRA
#endif
#if !defined(SWIFT_CLASS)
# if __has_attribute(objc_subclassing_restricted)
#  define SWIFT_CLASS(SWIFT_NAME) SWIFT_RUNTIME_NAME(SWIFT_NAME) __attribute__((objc_subclassing_restricted)) SWIFT_CLASS_EXTRA
#  define SWIFT_CLASS_NAMED(SWIFT_NAME) __attribute__((objc_subclassing_restricted)) SWIFT_COMPILE_NAME(SWIFT_NAME) SWIFT_CLASS_EXTRA
# else
#  define SWIFT_CLASS(SWIFT_NAME) SWIFT_RUNTIME_NAME(SWIFT_NAME) SWIFT_CLASS_EXTRA
#  define SWIFT_CLASS_NAMED(SWIFT_NAME) SWIFT_COMPILE_NAME(SWIFT_NAME) SWIFT_CLASS_EXTRA
# endif
#endif

#if !defined(SWIFT_PROTOCOL)
# define SWIFT_PROTOCOL(SWIFT_NAME) SWIFT_RUNTIME_NAME(SWIFT_NAME) SWIFT_PROTOCOL_EXTRA
# define SWIFT_PROTOCOL_NAMED(SWIFT_NAME) SWIFT_COMPILE_NAME(SWIFT_NAME) SWIFT_PROTOCOL_EXTRA
#endif

#if !defined(SWIFT_EXTENSION)
# define SWIFT_EXTENSION(M) SWIFT_PASTE(M##_Swift_, __LINE__)
#endif

#if !defined(OBJC_DESIGNATED_INITIALIZER)
# if __has_attribute(objc_designated_initializer)
#  define OBJC_DESIGNATED_INITIALIZER __attribute__((objc_designated_initializer))
# else
#  define OBJC_DESIGNATED_INITIALIZER
# endif
#endif
#if !defined(SWIFT_ENUM_ATTR)
# if defined(__has_attribute) && __has_attribute(enum_extensibility)
#  define SWIFT_ENUM_ATTR(_extensibility) __attribute__((enum_extensibility(_extensibility)))
# else
#  define SWIFT_ENUM_ATTR(_extensibility)
# endif
#endif
#if !defined(SWIFT_ENUM)
# define SWIFT_ENUM(_type, _name, _extensibility) enum _name : _type _name; enum SWIFT_ENUM_ATTR(_extensibility) SWIFT_ENUM_EXTRA _name : _type
# if __has_feature(generalized_swift_name)
#  define SWIFT_ENUM_NAMED(_type, _name, SWIFT_NAME, _extensibility) enum _name : _type _name SWIFT_COMPILE_NAME(SWIFT_NAME); enum SWIFT_COMPILE_NAME(SWIFT_NAME) SWIFT_ENUM_ATTR(_extensibility) SWIFT_ENUM_EXTRA _name : _type
# else
#  define SWIFT_ENUM_NAMED(_type, _name, SWIFT_NAME, _extensibility) SWIFT_ENUM(_type, _name, _extensibility)
# endif
#endif
#if !defined(SWIFT_UNAVAILABLE)
# define SWIFT_UNAVAILABLE __attribute__((unavailable))
#endif
#if !defined(SWIFT_UNAVAILABLE_MSG)
# define SWIFT_UNAVAILABLE_MSG(msg) __attribute__((unavailable(msg)))
#endif
#if !defined(SWIFT_AVAILABILITY)
# define SWIFT_AVAILABILITY(plat, ...) __attribute__((availability(plat, __VA_ARGS__)))
#endif
#if !defined(SWIFT_DEPRECATED)
# define SWIFT_DEPRECATED __attribute__((deprecated))
#endif
#if !defined(SWIFT_DEPRECATED_MSG)
# define SWIFT_DEPRECATED_MSG(...) __attribute__((deprecated(__VA_ARGS__)))
#endif
#if __has_feature(attribute_diagnose_if_objc)
# define SWIFT_DEPRECATED_OBJC(Msg) __attribute__((diagnose_if(1, Msg, "warning")))
#else
# define SWIFT_DEPRECATED_OBJC(Msg) SWIFT_DEPRECATED_MSG(Msg)
#endif
#if __has_feature(modules)
#if __has_warning("-Watimport-in-framework-header")
#pragma clang diagnostic ignored "-Watimport-in-framework-header"
#endif
@import ARKit;
@import CoreGraphics;
@import Foundation;
@import ObjectiveC;
@import SceneKit;
@import UIKit;
#endif

#import <ScapeKit/ScapeKit.h>

#pragma clang diagnostic ignored "-Wproperty-attribute-mismatch"
#pragma clang diagnostic ignored "-Wduplicate-method-arg"
#if __has_warning("-Wpragma-clang-attribute")
# pragma clang diagnostic ignored "-Wpragma-clang-attribute"
#endif
#pragma clang diagnostic ignored "-Wunknown-pragmas"
#pragma clang diagnostic ignored "-Wnullability"

#if __has_attribute(external_source_symbol)
# pragma push_macro("any")
# undef any
# pragma clang attribute push(__attribute__((external_source_symbol(language="Swift", defined_in="ScapeKit",generated_declaration))), apply_to=any(function,enum,objc_interface,objc_category,objc_protocol))
# pragma pop_macro("any")
#endif





@class SCNNode;
@class SCKLatLng;

/// (public) -
/// An SCKGeoAnchor keeps an underlying SCNNode attached to it as well as its local scenePosition
/// in the scene and worldLatLng in the World
SWIFT_PROTOCOL("_TtP8ScapeKit12SCKGeoAnchor_")
@protocol SCKGeoAnchor
/// (public) -
/// Unique id
@property (nonatomic, readonly, copy) NSString * _Nonnull uuid;
/// (public) -
/// Underlying node attached to this anchor
@property (nonatomic, readonly, strong) SCNNode * _Nonnull node;
/// (public) -
/// Worldwide latitude and longitude
@property (nonatomic, readonly, strong) SCKLatLng * _Nullable worldLatLng;
/// (public) -
/// Worldwide altitude
@property (nonatomic, readonly) double worldAltitude;
/// (public) -
/// Local Scene Position
@property (nonatomic, readonly) SCNVector3 scenePosition;
@end


/// (public) -
/// The SCKGeoAnchorManager protocol keeps a list of all SCKGeoAnchor and the nodes attached to them
/// in the scene and allow converting between local and world coordinates
SWIFT_PROTOCOL("_TtP8ScapeKit19SCKGeoAnchorManager_")
@protocol SCKGeoAnchorManager
/// (public) -
/// list of all SCKGeoAnchor that were registered when calling SCKGeoSceneView’s add method
@property (nonatomic, readonly, copy) NSArray<id <SCKGeoAnchor>> * _Nonnull geoAnchors;
/// (public) -
/// Convert world coordinates to local position -
/// Return nil if no measurements have been received: wait for onGeoSceneReady before calling this method
- (NSArray<NSNumber *> * _Nullable)worldLatLngToLocalPositionWithLatLng:(SCKLatLng * _Nonnull)latLng altitude:(double)altitude SWIFT_WARN_UNUSED_RESULT;
/// (public) -
/// Convert local position to world coordinates -
/// Return nil if no measurements have been received: wait for onGeoSceneReady before calling this method
- (SCKLatLng * _Nullable)localPositionToWorldLatLngWithPosition:(SCNVector3)position SWIFT_WARN_UNUSED_RESULT;
@end

@protocol SCKGeoSceneView;
@protocol SCKGeoArSessionObserver;
@class ARCamera;
@class ARFrame;

/// (public) -
/// The SCKGeoArSession protocol is the entry point to ScapeKit’s location-based AR features
/// It gives the access to the SCKGeoSceneView instance after starting the session via the “start” method
SWIFT_PROTOCOL("_TtP8ScapeKit15SCKGeoArSession_")
@protocol SCKGeoArSession
/// (public) -
/// Use this when you want to freeze whatever World LatLng and local coordinates your node already have
@property (nonatomic) BOOL freezeNodesWorldPositions;
/// (public) -
/// the SCKGeoSceneView instance to add any kind of node to the World using Latitude and Longitude
@property (nonatomic, readonly, strong) id <SCKGeoSceneView> _Nullable geoSceneView;
/// (public) -
/// start the SCKGeoArSession (mandatory to be able to add nodes) -
/// wait for SCKGeoArSessionObserver’s onGeoSceneReady callback before trying to add nodes to SCKGeoSceneView
- (void)startWithObserver:(id <SCKGeoArSessionObserver> _Nonnull)observer;
/// (public) -
/// stop the SCKGeoArSession -
/// this will both stop the SCKScapeSession measurements fetching as well as pause the ARKit’s session
- (void)stop;
/// (public) -
/// reset the SCKGeoArSession -
/// similar to start with the main difference being that this will reset ARKit’s tracking -
/// (Note: this will not remove the SCKGeoAnchors and their attached SCKGeoNodes from the scene.
/// For that you need to call manually SCKGeoSceneView’s removeAll()
- (void)resetWithObserver:(id <SCKGeoArSessionObserver> _Nonnull)observer;
/// (public) -
/// similar to start(obsever:) but using closures
- (void)startWithGeoSceneReady:(void (^ _Nonnull)(void))geoSceneReady geoAnchorAdded:(void (^ _Nonnull)(id <SCKGeoAnchor> _Nonnull))geoAnchorAdded geoSceneUpdated:(void (^ _Nonnull)(id <SCKGeoSceneView> _Nullable))geoSceneUpdated geoSceneTouched:(void (^ _Nonnull)(SCNNode * _Nonnull))geoSceneTouched geoArSessionError:(void (^ _Nonnull)(NSString * _Nonnull))geoArSessionError trackingStateChanged:(void (^ _Nonnull)(ARCamera * _Nonnull))trackingStateChanged frameUpdated:(void (^ _Nonnull)(ARFrame * _Nonnull))frameUpdated;
/// (public) -
/// similar to reset(obsever:) but using closures
- (void)resetWithGeoSceneReady:(void (^ _Nonnull)(void))geoSceneReady geoAnchorAdded:(void (^ _Nonnull)(id <SCKGeoAnchor> _Nonnull))geoAnchorAdded geoSceneUpdated:(void (^ _Nonnull)(id <SCKGeoSceneView> _Nullable))geoSceneUpdated geoSceneTouched:(void (^ _Nonnull)(SCNNode * _Nonnull))geoSceneTouched geoArSessionError:(void (^ _Nonnull)(NSString * _Nonnull))geoArSessionError trackingStateChanged:(void (^ _Nonnull)(ARCamera * _Nonnull))trackingStateChanged frameUpdated:(void (^ _Nonnull)(ARFrame * _Nonnull))frameUpdated;
@end


/// (public) -
/// A SCKGeoArSessionObserver handles SCKGeoArSession state changes
SWIFT_PROTOCOL("_TtP8ScapeKit23SCKGeoArSessionObserver_")
@protocol SCKGeoArSessionObserver
/// (public) -
/// Wait for this callback before trying to add nodes to SCKGeoSceneView
- (void)onGeoSceneReady:(id <SCKGeoArSession> _Nullable)geoArSession;
/// (public) -
/// Fired when a measurement (DeviceLocationMeasurements or ScapeMeasurements depending
/// on SCKGeoArSession’s useDeviceLocationAsFallback usage) has been found and therefore the anchor
/// can be added to the SCKGeoSceneView according to its LatLng
- (void)onGeoAnchorAdded:(id <SCKGeoArSession> _Nullable)geoArSession geoAnchor:(id <SCKGeoAnchor> _Nonnull)geoAnchor;
/// (public) -
/// Fired when a new measurement (DeviceLocationMeasurements or ScapeMeasurements depending
/// on SCKGeoArSession’s useDeviceLocationAsFallback usage) has been found and therefore the anchor
/// LatLng and scenePosition have been updated
/// (Note: use SCKGeoArSession’s stop() to not receive this event)
- (void)onGeoSceneUpdated:(id <SCKGeoArSession> _Nullable)geoArSession geoSceneView:(id <SCKGeoSceneView> _Nullable)geoSceneView;
/// (public) -
/// Fired whenever any kind of error has occured
- (void)onGeoArSessionError:(id <SCKGeoArSession> _Nullable)geoArSession errorMessage:(NSString * _Nonnull)errorMessage;
@optional
/// (public) -
/// Fired when a SCNNode has been touched
- (void)onGeoSceneTouched:(id <SCKGeoArSession> _Nullable)geoArSession touchedNode:(SCNNode * _Nonnull)touchedNode;
/// (public) -
/// ARKit’s callback wrapper
- (void)onTrackingStateChanged:(id <SCKGeoArSession> _Nullable)geoArSession camera:(ARCamera * _Nonnull)camera;
/// (public) -
/// ARKit’s callback wrapper
- (void)onFrameUpdated:(id <SCKGeoArSession> _Nullable)geoArSession frame:(ARFrame * _Nonnull)frame;
@end


/// (public) -
/// The SCKGeoCamera protocol represents the geo camera used for the entire scene.
/// As it is at the root of scene, this camera controls the transform of the whole scene
/// via the World Transform
SWIFT_PROTOCOL("_TtP8ScapeKit12SCKGeoCamera_")
@protocol SCKGeoCamera
/// (public) -
/// The camera node
@property (nonatomic, readonly, strong) SCNNode * _Nonnull cameraNode;
/// (public) -
/// The world transform node
@property (nonatomic, readonly, strong) SCNNode * _Nonnull worldTransformNode;
@end

@class SCNTechnique;
@class SCNHitTestResult;

/// (public) -
/// The SCKGeoSceneRenderer protocol wraps the underlying SCNSceneRenderer used for rendering the ARKit’s
/// session’s scene. It exposes the necessary SCNSceneRenderer’s methods
SWIFT_PROTOCOL("_TtP8ScapeKit19SCKGeoSceneRenderer_")
@protocol SCKGeoSceneRenderer
/// Sets or gets the current technique used for the underlying SCNSceneRenderer
@property (nonatomic, strong) SCNTechnique * _Nonnull sceneTechnique;
/// (public) -
/// Uses the underlying SCNSceneRenderer to return an array of SCNHitTestResult for each node that contains a specified point.
- (NSArray<SCNHitTestResult *> * _Nonnull)hitTestFromSceneRendererWithPoint:(CGPoint)point options:(NSDictionary<SCNHitTestOption, id> * _Nullable)options SWIFT_WARN_UNUSED_RESULT;
/// Uses the underlying SCNSceneRenderer to prepare the specified objects for drawing.
- (BOOL)prepareWithObject:(id _Nonnull)object shouldAbortBlock:(BOOL (^ _Nullable)(void))block SWIFT_WARN_UNUSED_RESULT;
/// Uses the underlying SCNSceneRenderer to prepare the specified objects for drawing on the background.
- (void)prepareWithObjects:(NSArray * _Nonnull)objects completionHandler:(void (^ _Nullable)(BOOL))completionHandler;
@end

@class UIView;
@class SCNScene;
@class UIColor;
@class SKVideoNode;
@class ARHitTestResult;

/// (public) -
/// The SCKGeoSceneView protocol is the entry point to ScapeKit’s location-based AR features
/// It allows adding nodes to its underlying scene using the node’s LatLng
SWIFT_PROTOCOL("_TtP8ScapeKit15SCKGeoSceneView_")
@protocol SCKGeoSceneView
/// (public) -
/// The underlying ARSCNView wrapped as UIView to avoid modifying ARSCNView’s scene or session configuration
@property (nonatomic, readonly, strong) UIView * _Nonnull nativeView;
/// (public) -
/// The underlying ARFrame
@property (nonatomic, readonly, strong) ARFrame * _Nullable currentFrame;
/// (public) -
/// Show debug options
@property (nonatomic) BOOL showDebugOptions;
/// (public) -
/// The current SCKGeoCamera representing the scene’s camera used for the entire scene.
@property (nonatomic, readonly, strong) id <SCKGeoCamera> _Nullable geoCamera;
/// (public) -
/// The current SCKGeoSceneRenderer representing the underlying SCNSceneRenderer
@property (nonatomic, readonly, strong) id <SCKGeoSceneRenderer> _Nullable geoSceneRenderer;
/// (public) -
/// The current scene representing the underlying ARKit’s session SCNScene
@property (nonatomic, readonly, strong) SCNScene * _Nonnull nativeScene;
/// (public) -
/// The SCKGeoAnchor’s manager: holds a list of all SCKGeoAnchor added to the scene
@property (nonatomic, readonly, strong) id <SCKGeoAnchorManager> _Nullable geoAnchorManager;
/// (public) -
/// Add SCNNode to the scene using LatLng and altitude by attaching a SCKGeoAnchor to it
- (void)addWithNode:(SCNNode * _Nonnull)node latLng:(SCKLatLng * _Nonnull)latLng altitude:(double)altitude;
/// (public) -
/// Add any kind of UIView except WKWebView
/// the scale parameter allows you to make the view bigger or smaller depending on how far you are
- (SCNNode * _Nonnull)addWithView:(UIView * _Nonnull)view scale:(CGFloat)scale latLng:(SCKLatLng * _Nonnull)latLng altitude:(double)altitude SWIFT_WARN_UNUSED_RESULT;
/// (public) -
/// Add webview to the scene based on the given url
- (SCNNode * _Nullable)addWebViewWithUrl:(NSString * _Nonnull)url width:(CGFloat)width height:(CGFloat)height scale:(CGFloat)scale latLng:(SCKLatLng * _Nonnull)latLng altitude:(double)altitude SWIFT_WARN_UNUSED_RESULT;
/// (public) -
/// Add 3d text label
/// the depth parameter makes the text thicker
- (SCNNode * _Nonnull)addLabelWithLabel:(NSString * _Nonnull)label fontSize:(CGFloat)fontSize fontColor:(UIColor * _Nonnull)fontColor fontName:(NSString * _Nonnull)fontName depth:(CGFloat)depth latLng:(SCKLatLng * _Nonnull)latLng altitude:(double)altitude SWIFT_WARN_UNUSED_RESULT;
/// (public) -
/// Add video to the scene based on the given url
- (SKVideoNode * _Nullable)addVideoWithUrl:(NSString * _Nonnull)url latLng:(SCKLatLng * _Nonnull)latLng altitude:(double)altitude SWIFT_WARN_UNUSED_RESULT;
/// (public) -
/// Remove SCNNode and its attached SCKGeoAnchor from the SCKGeoSceneView
- (void)removeWithNode:(SCNNode * _Nonnull)node;
/// (public) -
/// Remove all SCNNodes and their attached SCKGeoAnchors from the SCKGeoSceneView
- (NSArray<SCNNode *> * _Nonnull)removeAll SWIFT_WARN_UNUSED_RESULT;
/// (public) -
/// Searches the current frame for objects corresponding to a point in the view.
- (NSArray<ARHitTestResult *> * _Nonnull)hitTestOnCurrentFrameWithPoint:(CGPoint)point types:(ARHitTestResultType)types SWIFT_WARN_UNUSED_RESULT;
@end



@class CLLocation;

@interface SCKLocationMeasurements (SWIFT_EXTENSION(ScapeKit))
/// (public) -
/// Convert Scape-based device location measurements to CLLocation
- (CLLocation * _Nonnull)toCLocation SWIFT_WARN_UNUSED_RESULT;
@end

@protocol SCKScapeClientBuilder;

/// (public) -
/// The SCKScape class is the entry point to use ScapeKit.
SWIFT_CLASS("_TtC8ScapeKit8SCKScape")
@interface SCKScape : NSObject
- (nonnull instancetype)init SWIFT_UNAVAILABLE;
+ (nonnull instancetype)new SWIFT_UNAVAILABLE_MSG("-init is unavailable");
/// (public) -
/// The SCKScapeClientBuilder instance. Entry point to SCKScapeClient configuration
SWIFT_CLASS_PROPERTY(@property (nonatomic, class, readonly, strong) id <SCKScapeClientBuilder> _Nonnull scapeClientBuilder;)
+ (id <SCKScapeClientBuilder> _Nonnull)scapeClientBuilder SWIFT_WARN_UNUSED_RESULT;
/// (public) -
/// ScapeKit’s current version.
SWIFT_CLASS_PROPERTY(@property (nonatomic, class, readonly, copy) NSString * _Nonnull sdkVersion;)
+ (NSString * _Nonnull)sdkVersion SWIFT_WARN_UNUSED_RESULT;
@end

@class SCKScapeSession;
@protocol SCKDebugSession;
@protocol SCKScapeClientObserver;

/// (public) -
/// The SCKScapeClient protocol is ScapeKit’s entry point.
/// It provides access to the feature classes in the SDK: SCKScapeSession, SCKGeoArSession and SCKDebugSession.
SWIFT_PROTOCOL("_TtP8ScapeKit14SCKScapeClient_")
@protocol SCKScapeClient
/// (public) -
/// True if SCKScapeClient instance is already started
@property (nonatomic, readonly) BOOL isStarted;
/// (public) -
/// SCKScapeSession instance giving access to all Device and Scape measurements
/// ScapeKit provides.(Nil if start(observer: ) is not called or fails)
@property (nonatomic, readonly, strong) SCKScapeSession * _Nullable scapeSession;
/// (public) -
/// SCKGeoArSession instance giving access to location-based AR features
/// ScapeKit provides. (Nil if start(observer: ) is not called or fails)
@property (nonatomic, readonly, strong) id <SCKGeoArSession> _Nullable geoArSession;
/// (public) -
/// SCKDebugSession instance giving access to debugging features
/// ScapeKit provides. (Nil if withDebugSupport(true) is not set or start(observer: ) is not called)
@property (nonatomic, readonly, strong) id <SCKDebugSession> _Nullable debugSession;
/// (public) -
/// Starts SCKScapeClient. If it fails or not called, all instances (debugSession, scapeSession, geoArSession) are nil.
/// Wait for SCKScapeClientObserver’s onClientStarted before retreiving any of SCKScapeClient’s instance
- (void)startWithObserver:(id <SCKScapeClientObserver> _Nonnull)observer;
/// (public) -
/// Stops SCKScapeClient. Stops all sessions
- (void)stopWithObserver:(id <SCKScapeClientObserver> _Nonnull)observer;
/// (public) -
/// Terminates SCKScapeClient. All instances become invalid after calling it
- (void)terminateWithObserver:(id <SCKScapeClientObserver> _Nonnull)observer;
/// (public) -
/// Starts SCKScapeClient. If it fails or not called, all instances (debugSession, scapeSession, geoArSession) are nil
- (void)startWithClientStarted:(void (^ _Nonnull)(void))clientStarted clientFailed:(void (^ _Nonnull)(NSString * _Nonnull))clientFailed;
/// (public) -
/// Stops SCKScapeClient. Stops all sessions
- (void)stopWithClientStopped:(void (^ _Nonnull)(void))clientStopped clientFailed:(void (^ _Nonnull)(NSString * _Nonnull))clientFailed;
/// (public) -
/// Terminates SCKScapeClient. All instances become invalid after calling it
- (void)terminateWithClientStopped:(void (^ _Nonnull)(void))clientStopped clientFailed:(void (^ _Nonnull)(NSString * _Nonnull))clientFailed;
@end


/// (public) -
/// The SCKScapeClientBuilder protocol builds a new SCKScapeClient instance.
/// To construct a SCKScapeClient, the required configuration parameters are:
/// <ul>
///   <li>
///     Application Key
///     It is optional to specify:
///   </li>
///   <li>
///     Debug Support (for console logs and visual logs)
///   </li>
/// </ul>
SWIFT_PROTOCOL("_TtP8ScapeKit21SCKScapeClientBuilder_")
@protocol SCKScapeClientBuilder
/// (public) -
/// Sets the SCKScapeClient’s api key. If empty SCKScapeClient’s start will fail
- (id <SCKScapeClientBuilder> _Nonnull)withApiKey:(NSString * _Nonnull)apiKey SWIFT_WARN_UNUSED_RESULT;
/// (public) -
/// When set to true, SCKDebugSession will be instantiated and will allow access to logging tools
/// as well controlling the overlay console
- (id <SCKScapeClientBuilder> _Nonnull)withDebugSupport:(BOOL)isSupported SWIFT_WARN_UNUSED_RESULT;
/// (public) -
/// Creates the resulting SCKScapeClient.
- (id <SCKScapeClient> _Nonnull)build SWIFT_WARN_UNUSED_RESULT;
@end


/// (public) -
/// A SCKScapeClientObserver handles client state changes
SWIFT_PROTOCOL("_TtP8ScapeKit22SCKScapeClientObserver_")
@protocol SCKScapeClientObserver
/// (public) -
/// Notifies that SCKScapeClient has effectively started
- (void)onClientStarted:(id <SCKScapeClient> _Nonnull)scapeClient;
/// (public) -
/// Notifies that SCKScapeClient has effectively stopped
- (void)onClientStopped:(id <SCKScapeClient> _Nonnull)scapeClient;
/// (public) -
/// Notifies that SCKScapeClient has stopped
- (void)onClientFailed:(id <SCKScapeClient> _Nonnull)scapeClient errorMessage:(NSString * _Nonnull)errorMessage;
@end


@interface SCKScapeOrientation (SWIFT_EXTENSION(ScapeKit))
/// (public) -
/// Convert ScapeKit orientation to an actual SceneKit Quaternion
- (SCNQuaternion)toSNQuaternion SWIFT_WARN_UNUSED_RESULT;
/// (public)
/// Retrieve yaw from ScapeKit orientation
- (CGFloat)yaw SWIFT_WARN_UNUSED_RESULT;
/// (public)
/// Retrieve pitch from ScapeKit orientation
- (CGFloat)pitch SWIFT_WARN_UNUSED_RESULT;
/// (public)
/// Retrieve roll from ScapeKit orientation
- (CGFloat)roll SWIFT_WARN_UNUSED_RESULT;
@end







@class SCKMotionMeasurements;
@class SCKScapeMeasurements;
@class ARSession;

@interface SCKScapeSession (SWIFT_EXTENSION(ScapeKit))
/// (public) -
/// Get the measurements
/// Combine device’s sensors-based measurements (gyroscope, accelerometer, gps, magnetometer)
/// with Scape Vision Engine to get hyper-accurate measurements
- (void)getMeasurements:(void (^ _Nonnull)(double))scapeMeasurementsRequested :(void (^ _Nonnull)(SCKLocationMeasurements * _Nullable))deviceLocationMeasurementsUpdated :(void (^ _Nonnull)(SCKMotionMeasurements * _Nullable))deviceMotionMeasurementsUpdated :(void (^ _Nonnull)(SCKScapeMeasurements * _Nullable))scapeMeasurementsUpdated :(void (^ _Nonnull)(SCKScapeSessionState, NSString * _Nonnull))sessionError;
/// (public) -
/// Start measurements fetching at continuous interval
/// Combine device’s sensors-based measurements (gyroscope, accelerometer, gps, magnetometer)
/// with Scape Vision Engine to get hyper-accurate measurements
- (void)startFetch:(void (^ _Nonnull)(double))scapeMeasurementsRequested :(void (^ _Nonnull)(SCKLocationMeasurements * _Nullable))deviceLocationMeasurementsUpdated :(void (^ _Nonnull)(SCKMotionMeasurements * _Nullable))deviceMotionMeasurementsUpdated :(void (^ _Nonnull)(SCKScapeMeasurements * _Nullable))scapeMeasurementsUpdated :(void (^ _Nonnull)(SCKScapeSessionState, NSString * _Nonnull))sessionError;
/// (public) -
/// Needed for all AR related features (camera frame, camera transform, camera intrinsics)
- (void)setARSession:(ARSession * _Nonnull)arSession;
/// (public) -
/// Set the ar frame manually
- (void)setARFrame:(ARFrame * _Nonnull)arFrame;
@end



















#if __has_attribute(external_source_symbol)
# pragma clang attribute pop
#endif
#pragma clang diagnostic pop
