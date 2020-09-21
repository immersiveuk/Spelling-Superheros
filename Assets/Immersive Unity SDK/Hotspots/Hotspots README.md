# Hotspots (iUS)

## Overview
A common way to make content for Immersive Spaces is using "Hotspots". These are triggers in the scene that when clicked trigger a common function, eg. a popup or changing scene. As Hotspots are so commonly used a set of tools are supplied which allow content creators to make Hotspot scenes very rapidly without any programming. These tools can be used in conjunction with any other Unity tools to make more advanced programs.

## Getting Started
When creating an Immersive Experience, create a new scene by choose Immersive Interactive in the Menu Bar and choosing either New 2D Scene or New 3D Scene depending on the type of scene you are making. When this is done you will have a scene with an Immersive Camera and a "Stage" object. This is an empty GameObject where you should put all visible GameObjects that you wish to appear on the walls. This "Stage" object has shortcut buttons on it which allow quickly create commonly used objects for Immersive Content. For a Hotspots scene you must add a Hotspot Controller by clicking the "New Hotspot Controller" button.

A Hotspot Controller is an empty GameObject of which controls a set of Hotspots which are children of the controller. The Hotspot controller will control when and how Hotspots are made visible to the user. Additionally the Hotspot Controller has shortcuts in it to quickly create new Hotspots of different types as well as a Hotspot Batch. Clicking one of these buttons will automatically add the desired object into the scene as a child of the Hotspot Controller.

## Hotspot Types
Any Unity GameObject can be a made into a Hotspot. For convenience there are a series of commonly used defaults types which can be quickly instantiated. These are:

+ Basic Hotspot - A Hotspot represented by a simple square.
+ Image Hotspot - A Hotspot which is visually represented as a sprite. You can change how the image looks by changing the properties of the Sprite Renderer component.
+ Invisible Hotspot - An invisible area which can act as a Hotspot. This is particularly useful if you wish to make parts of the background into a Hotspot. It works well when combined with a Blur and Desaturate effect.

## Hotspot Batch
It is possible to Batch hotspots togther so that they are revealed or highlighted simultaneously. This is done by creating a Hotspot Batch. A shortcut for this can be found in the Hotspot Controller. Any Hotspots which children of the Batch object are part of the same Batch. The Batch object has shortcuts to create new Hotspots.

## Hotspot Controller Settings
This is an overview of the settings you can change in the Hotspot Controller.

+ Reveal Type - This defines how and when Hotspots are activated and revealed. The options are as follows:
	+ AllAtOnce - All Hotspots are visible and active as soon as the scene loads.
	+ Ordered - Hotspots and Batches are revealed one at a time. The reveal order is determined by there order they appear in the scene hierarchy. A Batch of Hotspots are all revealed simultaneously.
	+ Highlight - All Hotspots are visible at the start of the scene however Hotspots and Batches will be activated one at a time, just like in Ordered mode. When Hotspots are activate they will have an outline which indicates that they can by interacted with.

+ Blur and Destaurate - Only an option in Ordered and Highlight modes. If this is true a blur and desaturate filter will be applied to the screens. This filter is stronger, further away from the Hotspot it is focussing on. This allows you to direct the users attention towards a specific Hotspot. This works well in conjunction with Invisible Hotspots. This effect does not work with a Batch and will just ignore any batches.
	+ Blur In Duration - This determines how long it takes for the Blur and Desaturate effect to reach maximum intensity.
	+ Blur Out Duration - This determines how long it takes for the Blur and Destaurate effect to fade out between Hotspots.
	
+ Single Hotspot Open At Once - If this settings is true, it means that if a Hotspot Popup is visible, due to a Hotspot being pressed, no other Hotspots can be pressed until the Popup has been dismissed.

## Hotspot Actions
There are a variety of different actions that a hotspot can do. The most common type is to present a Popup. Other functionality has the Hotspot act more like a button.

### Popup Types
+ Image - Displays a sprite.
+ Video - Can display a video from VideoClip or URL.
+ Text - Displays a user defined text box.
+ Q&A - Displays displays a questions and two possible answers. Users can choose which one they think is correct. 

### Other Hotspot Functionality
+ SceneLink - Changes to specified scene.
+ Activate and Hide Objects - Allows you to define objects to be enabled and disabled when the Hotspot is pressed.

## Hotspot Settings
By clicking on a Hotspot in the scene or hierarchy view you can change a series of settings which alter the functionality of the Hotspot. 

+ When Selected - This property determines what will happen to the Hotspot itself when it is clicked. 
	+ Delete - The Hotspot will be perminantly deleted.
	+ Hide - The Hotspot will be hidden until the popup is dismissed at which point it will reapear. 
	+ Disable - The Hotspot will remain visible but functionality will be disabled until the popup is dismissed.

### Action Settings

+ Play Audio On Touch - If true the specified Audio Clip will play when the Hotspot is touched.
	+ Audio Clip - The Audio Clip to play.
	+ Audio Volume - The Volume to play to Audio Clip at.

+ Action - What action you wish to do as described in the Hotspot Actions Settings. Each action type has its own set of specific settings.

#### Popup Positions
All Popup actions allow you to alter where the Popup should appear. These are as follows:

+ Surface Center - Popup appears in the middle of the surface the Hotspot is on.
	
``` 
    ┌──────────────────────────────────────────┐
    │                                          │
    │                                          │
    │ ┌────────┐                               │
    │ │Hotspot │   ┌───────────┐               │
    │ └────────┘   │           │               │
    │              │           │               │
    │              │   Popup   │               │
    │              │           │               │
    │              └───────────┘               │
    │                                          │
    │                                          │
    │                                          │
    └──────────────────────────────────────────┘
```
+ Same As Hotspot - Popup appears in the same position as the Hotspot which spawned it. If the popup overlaps with the edge of the surface, it will automatically be moved to fit in the surface.      
	                                                  
```
    ┌──────────────────────────────────────────┐
    │┌───────────┐                             │
    ││           │                             │
    ││           │                             │
    ││   Popup   │                             │
    ││           │                             │
    ││           │                             │
    │└───────────┘                             │
    │                                          │
    │                                          │
    │                                          │
    │                                          │
    │                                          │
    │                                          │
    └──────────────────────────────────────────┘
```                                                 
   + Custom - The a user defined position. The position is defined as an offset, in pixels, from the Hotspots position.

                                                     
```
      ┌─────────────────────┐  
      │   Pos: (200,-300)   │  
      └─────────────────────┘  

   
    ┌──────────────────────────────────────────┐
    │                                          │
    │                                          │
    │ ┌────────┐                               │
    │ │Hotsp┌──┴────────┐                      │
    │ └─────┤           │                      │
    │       │           │                      │
    │       │   Popup   │                      │
    │       │           │                      │
    │       │           │                      │
    │       └───────────┘                      │
    │                                          │
    │                                          │
    │                                          │
    └──────────────────────────────────────────┘
```


+ Type - The popup type as described above.

#### Image Pop-Up settings

+ Sprite - The sprite to display.
+ Resolution - Define the X and Y pixel values of the image.
    + Maintain Aspect Ratio - When changing the resolution, maintain the aspect ratio of the original image.
    + Reset Resolution - A button which will reset the resolution to the resolution of the original image.
+ Color - Modify the color of the sprite.

#### Image Sequence Pop-Up settings

+ Use Custom Buttons - If true you can provide custom sprites for the forward and backwards buttons.
+ Control Panel Style - This determines what buttons appear in the control panel. The options are:
    + Full - Next, previous and close buttons are all visible.
    + Forward and Close - Next and close buttons are visible.
    + Forward Only - Only a forward button is visible. Once you get to the final image in the sequence, the next button is replaced by a close button.

#### Video Pop-Up settings

+ Source - Whether the video is in the form of a URL or VideoClip.
+ URL - The URL of the video to display
+ VideoClip - The video clip to display.
+ Width - The width in pixels of the the popup. The height will be automatically calculated based on the aspect ratio.

#### Text Pop-Up settings

+ Text - The string you wish the popup to display.

#### Q&A Pop-Up settings

+ Question - The question asked.
+ Answer 1 - The first possible answer.
+ Answer 2 - The second possible answer.
+ Correct Answer - Which answer is the correct one.

#### Scene Link Settings

+ Scene - The name of the scene you wish to move to.
+ Fade To Black - If true the scene will fade to black before moving to the next scene.
+ Fade To Black Duration - How many seconds it should take for the scene to fade to black.


#### Activate And Hide Settings

+ Objects to Hide - A resizable array of objects to hide.
+ Objects to Reveal - A resizable array of objects to reveal.


## Multiple Hotspot Controllers.
It is possible to have multiple Hotspot Controllers in a single scene. They each act independently and will not interfere with each other. One use for this is having repeated functionality on each surface with independent controllers.