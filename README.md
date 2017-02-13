# Exo


![Exo](exo.jpg)

Exo is an immersive VR game where players pilot a futuristic drone around an unknown planet, working to discover and activate an ancient alien technology. Players will need to figure out how to interact with the world through their drone with help from a mothership hovering above. The drone’s automatic scanner gives some clues about the connection between objects in the world and hints at where to go next, but the rest is left to the player to discover.

This app is an open source Android Experiment created by [Hook Studios](http://byhook.com/)

[![axp](https://www.androidexperiments.com/assets/img/axpbadge.svg)](https://www.androidexperiments.com/experiment/exo)

[<img alt="Get it on Google Play" height="45px" src="https://play.google.com/intl/en_us/badges/images/apps/en-play-badge-border.png" />](https://play.google.com/store/apps/details?id=com.byhook.exo)

## Background

Exo was born out of a series of prototypes that Hook Studios did on a makeshift Daydream dev kit before any actual Daydream hardware was available. We were exploring possible uses of the Daydream controller’s unique rotational tracking and touchpad interface. An early prototype tried out the a simple drone flight mechanic that translated the controller’s head-relative orientation to the drone’s orientation with a simple on/off thrusting mechanism. This turned out to be both fun and challenging. The rest of the experience was built around iterations of that basic control concept, which is not possible on other mobile VR platforms. Once the Daydream hardware became available to test on, we added in variable thrusting using the player's finger position on the touchpad.

## Technology

The app was created in the ever-evolving Daydream Technical Preview version of Unity. It utilizes the Google VR SDK and some of the latest features in Unity 5 like physically based rendering, real-time global illumination, etc. Daydream’s async reprojection was key to keeping the frame rate up while pushing the visual fidelity.

The final version in this repo can be built using the following:

- [Unity Daydream Technical Preview 5.4.2f2-GVR13](https://unity3d.com/partners/google/daydream)
- [Google VR SDK v1.10.0](https://github.com/googlevr/gvr-android-sdk/releases/tag/v1.10.0) *(included in the source tree)*
- [Android SDK 7.0+](https://developer.android.com/studio/index.html)

**NOTE:** Make sure to set the Unity build platform to **'Android'** in **'Build Settings'** or the project won't run properly.

You can use an Android phone to [emulate a Daydream VR controller](https://developers.google.com/vr/daydream/hardware#the_controller_emulator) in the Unity editor.

## Credits

This application was created by [Hook Studios](http://byhook.com/)

#### Production
- [Annie Porter](https://www.linkedin.com/in/annie-porter/)

#### Unity Developers
- [James Vecore](https://github.com/jamesvecore)
- [Celso de Melo](https://github.com/CelsoDeMelo)
- [Brad Nawrocki](https://github.com/septinite)

#### Art
- [Christian Jang](https://christianjang.com)
- [Ryan Quinn](https://github.com/ryan-quinn)
- [Brad Fermin](https://vimeo.com/user8209034)

#### Music and Sounds
- [Mark Fain](https://github.com/hookmark)
