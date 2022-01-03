# Unity Onomatopoeia
Create and display onomatopoeia in Unity.
This package is a work in progress and not yet suitable for any production uses.
It was originally custom made for a project of mine, and I expanded it for more general use. Some relic of my precise use may still lurk around as I clean up the codebase.

## How to install
You can add this package to your Unity project by adding its Github URL in the package manager (`https://github.com/Sainna/UnityOnomatopoeia.git`)

## How to use
**This section is not yet up to date but gives a rough idea of the package.**

### Creating an onomatopoeia
An Onomatopoeia prefab should always have an empty parent (this is to facilitate rotation and GameObject management). To begin, create a gameobject with an empty parent and add the Onomatopoeia script.

![Onomatopoeia editor explanation](https://i.imgur.com/wlkp1p0.png "Onomatopoeia editor")

You can see on the picture above the values that are safe to edit.

Text color, font size/height/depth and animation speed all depends on the impact speed of the Collider. These values are multiplied by the *SpeedModifier*, *SizeModifier* and *HeightModifier* values. The *External* values of the modifiers are to edit these properties via an external script (Such as an additional effect).

The Climb Anim and Alpha Anim curves works as follow:


| Value in curve editor | Time axis | Value axis |
| -------- | -------- | -------- |
| 0     | Start of animation     | 0 |
| 1     | Base anim time (seconds)     | Editor's value * modifiers |

#### External animations
External animations allows you to add specific TextMeshPro animation to your text. You can add them through the drop down menu in the Onomatopoeia Editor.
Documentation Todo.

### Onomatopoeia via script (Onomatopoeia Generator)
The easiest way to use the plugin is via the OnomatopoeiaGenerator script. Select the Onomatopoeia prefab to instantiate and call OnomatopoeiaAt function.

### Add an onomatopoeia on object hit

To add a new Onomatopoeia generator, add the `ColliderImpact` script to your GameObject. Here are the different options:
![](https://i.imgur.com/RC0J4Q8.png)


If you want to have the effect fire upon colliding with another object, don't forget to also add a collider.


#### Per-collider sound
Using Per-collider sound effects allows for better control over each sounds. To use it, in addition to the `ColliderImpact`, add an `AudioSource` and an `ColliderImpactSound` script. Set up their base references appropriately.

On the `ColliderImpactSound` script, add the different sound effect you want in the *AudioClips* array. One will be picked at random and played on impact.

#### Per-onomatopoeia sound
To use a Per-Onomatopoeia sound, ignore the `ColliderImpactSound` setup and simply add an AudioSource to your Onomatopoeia prefab. Do not use Play on Awake, as the script will Play the file automatically.


### External effects and modifiers for text effects
External effects and modifiers can be created with a **prefab** containing the ***Text External Effect*** script. 

These external effects prefab can also contain speech bubbles and Particle systems, or anything you would want to be displayed with the text. 

The scripts value should be self-explanatory. Any value containing "Multiplier" will only change the Onomatopoeia *External○○* values. Everything else is an override of the default onomatopoeia, except the TM Anims array if the *Cancel Other Anims* property is not ticked.

As an example, here is the Water Effect prefab:
![](https://i.imgur.com/BJKNp4G.png)