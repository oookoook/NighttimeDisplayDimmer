# Help and FAQ

Here you can find the answers to the most often asked questions.

## How does the Sunset Dimmer work?

The app works as companion to the [Windows 10 Night mode feature](https://support.microsoft.com/en-us/windows/set-your-display-for-night-time-in-windows-18fe903a-e0a1-8326-4c68-fd23d7aaf136). If you enable this feature and install this app, 
it will detect the Night mode was switched on or off and adjust your display brightness according to your preffered settings.

## Is my display supported?

I can't be sure, but probably yes. Generally all laptop displays can be controlled by the app and most of the standalone displays, event old ones (even 10 years old if they are connected by digital interface - DVI-D, HDMI, DP), can be controlled.

Just make sure to enable *DDC/CI* in your display on-screen menu.

Unfortunatelly, some connector/signal converters do not support passing DDC/CI communication.

To help you verify display compatibility, the app offers [two weeks trial](https://TODO), no strings attached. 

## What Windows versions are supported?

Windows 10 version 1903 (May 2019 Update, build 10.0.18362) and up. This app was unfortunatelly NOT tested on Windows ARM.

## How to use the app?

* Make sure you have the [Night Mode Windows feature](https://support.microsoft.com/en-us/windows/set-your-display-for-night-time-in-windows-18fe903a-e0a1-8326-4c68-fd23d7aaf136) enabled and set up properly (I recommend enabling the automatic schedule).
* The app starts as an icon in the Windows toolbar.
* Start by double clicking the app icon.
* Wait for the app to load your supported displays. If you don't see your display try to go through the display on-screen menu and enable the *DDC/CI* feature and close and repoen the app window.
* Use the toggle switches to enable control of the displays you want to dim.
* After you enable the display, two sliders and two toglge switches (see below) show up.
* Use the slider to set the desired brightness levels. Alternatively, you can use your display brightness controls (e.g. OSD menu od hotkeys on your laptop keyboard) and then use the *Set current brightness* button below to use the current brightness as Day or Night presset.
* Save the configuration using the *Save* button.
* Test the configuration by switching the *Night mode* on or off using the button in Windows notification panel. After toggling the Night mode, the app will automatically adjust the brightness in a few seconds.
* A toast notification should pop in your notification bar. You can disable the notifications using the setting in the Sunset Dimmer options window located under the display list.

## What are the Force increase/decrease options good for?

Imagine the following situation: You have the Sunset dimmer configured to dim your laptop display to 30% during night, because in your typical workplace, there is some level of ambient lightning. Now you are travelling and you have manually adjusted your display brightness to the minimum to prolong the battery life. 
Now when the Night mode turns on, the desired brightness is 30%, but the actual brightness is 0% and the brightness would *increase*.

Very similar rule applies to the day: If the day preset is set to 75%, but you manually increased the brightness to 100% (e.g. you are on vacation and working in the morning near a window with direct sunlight shining on the display), the automatic change would *decrease* the brightness. 

This is generally not desired behavior and Sunset Dimmer won't change the brightnes in such cases. But you can force the app to change brightness every time if your use case is different.

## Can I get the app for free?

> Short answer: *Yes*. Is it worth it? Probably no.

This app is Open Source, that means you can use the source code in [GitHub repository](https://github.com/oookoook/NighttimeDisplayDimmer) to build the app and use it completely free of charge. In my opinion, your time is probably worth more that is the price of the app in Windows Store even if you know what you are doing. 
If you dont have experience with software development and .NET framework, this will probably be quite a rocky road.

## How can I report an issue or ask for help?

Either [create an GitHub issue] or send an e-mail to [sunsetdimmer@nastojte.cz](mailto:sunsetdimmer@nastojte.cz)