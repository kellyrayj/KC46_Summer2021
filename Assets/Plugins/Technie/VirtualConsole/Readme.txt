
Technie Virtual Console 2.0
===========================

Hello! Welcome to the Technie Virtual Console. Version 2.0 has had a bit of an overhaul, so even if you've already used it then please watch the introduction video and read this readme for more information on setup and changes.

Setup
-----

To add the virtual console to your project simply add the 'Virtual Console' prefab (from `Assets/Technie/VirtualConsole/Prefabs/Virtual Console.prefab`) to your scene.

It is assumed that you already have your project set up for VR and you have enabled VR support in the Unity Player Settings. The Virtual Console will work on any platform with Unity XR and hand controllers - that includes HTC Vive, Oculus Rift, Oculus Quest, PlayStation VR and WindowsMR.

Camera Detection
----------------
By default the console is set up to detect your vr camera automatically. However if you have multiple VR cameras you may want to specific which vr camera to use manually. Do this via the 'Camera Detection' settings in the inspector on the root of the prefab (the Virtual Console component).

 * 'Automatic' uses the first active and vr-enabled camera in the scene. This will be fine for most projects.
 * 'Use Explicit Camera Reference' allows you to drag in a camera to use (useful if the camera is in the same scene)
 * 'Use Explicit Camera Name' allows you to set the name of the camera to use (useful if the camera is in a different scene or a prefab that's loaded later)

Example Scene
-------------

There's an example scene in `Assets/Technie/VirtualConsole/Scenes/Example1.scene`. This contains the Virtual Console prefab, the SteamVR camera prefab, a minimal environment, and some trigger boxes. If you are not using SteamVR then delete the [CameraRig] prefab and replace it with the Oculus Rift camera rig from the Rift SDK instead.

Putting a hand inside the trigger boxes will trigger different kinds of log/stat information to be created to demonstrate the Virtual Console.


Functionality
-------------

The 'Virtual Console' prefab can be included normally within your scene, and enabled/disabled at runtime by enabling/disabling the root game object. All customisation options are exposed on the `VirtualConsole` component on the root of the prefab.

By default the console will delete itself if running in a release build (ie. 'Development Build' in Build Settings dialog is unticked). This functionality can be turned off with the `Only in debug builds` option on the VirtualConsole component.

If you would rather not have any additional loading overhead for release builds then you can spawn the Virtual Console prefab dynamically as your game loads.

### General Controls ###

A virtual console panel will appear attached to each hand, with stats and information on it. Each hand will also have a 'stylus' attached to it (a small blue sphere). Use this to poke the buttons on the panels to press them. Version 2.0 does not use any button input so that it does not interfere with your game's normal input handling.

Every panel has a few common controls:

- The current panel name is shown at the top. Use the left and right arrows next to it to change the current panel to a different one.
- The 'minimise' button ('_') next to this can collapse the panel down to a small rectangle. This is useful to hide it if you want to capture a screenshot or if it's getting in the way.
- The 'maximise' button ('+') at the top right will toggle the panel between small and large sizes. This is especially useful if you have a lot of console text that you need to see and the small panel can't show it all.
- When minimised, the panel will be replaced with a 'restore' button which will return it to it's previous size.

### Console Panel ###

This displays messages sent to Unity's console via the usual `Debug.Log()`, `Debug.LogWarning()` and `Debug.LogError()`. Uncaught exceptions will also be shown, with the stack trace reformatted to be more space efficient.

The buttons at the bottom can be used to toggle visibility of info, warnings and errors separately. The 'Clear' button will clear all log messages shown on the VR console.

### Stats Panel ###

Stats are a different way of inspecting internal game state. They are paticularly useful for watching a fast changing internal values - such as what state a state machine is in, or internal cooldown values for gameplay mechanic. Values which if output to Debug.Log would heavily spam the console and hide other output.

These are logged by calling `VrDebugStats.SetStat()`. Stats are a name with an associated value (plus an optional category). Setting a stat either creates a new one if one does not already exist, or overwrites the existing value if one does exist.

Pressing the 'Clear' button on the vr panel will clear all stats. The arrows can be used to cycle through different categories of stats.

For an example usage, see `Assets/Technie/VirtualConsole/Scripts/Example/InfoExample.cs`

### Auto Break Panel ###

The auto break panel effectively lets you press the 'pause' button in Unity, but without having to leave VR. This is especially useful if you're trying to debug something that requires holding an object or a controller in the air and you can't reach your keyboard (or have run out of hands).

The 'Delayed Pause' is the same, but has a timer before it pauses (like a selfie timer on a camera). Set how long you want the delay to be, then press 'Delayed Pause'. You now have a few seconds to pick up that thing, position your head and hands to recreate the bug you're trying to fix, etc. before Unity will pause and you can take the headset off to debug it.


Stylus Locations
----------------

By default the styluses attached to your hands will appear next to your thumbs. Depending on your game or control scheme, you may want them elsewhere. On the root of the Virtual Console prefab you can choose from several pre-set stylus positions. Experiment and see which works best for you.

If you need to you can specify exactly where the stylus goes. Choose `Custom` from the drop down, and then enter the position you want them to appear in the 'Custom Stylus Position' boxes. These positions are relative to the origin of the controllers.


Problems? Feature Requests? Bugs?
---------------------------------

Send an email to `technie@triangularpixels.com` for support and feature suggestions.

Please include your Unity version, OS and VR platform in any support emails. If reporting a bug then if you include a (small!) reproduction project with instructions on how to reproduce your bug then we'll be able to fix things *much* quicker. Thanks!





