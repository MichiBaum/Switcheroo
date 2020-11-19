# <img src="logo.png" alt="Switcheroo" width="48px" height="48px"> Switcheroo ![GPL License](https://img.shields.io/badge/license-GPL-brightgreen.svg)

Switcheroo is for anyone who spends more time using a keyboard than a mouse.
Instead of alt-tabbing through a (long) list of open windows, Switcheroo allows
you to quickly switch to any window by typing in just a few characters of its title.

This is a improved and extended version of [this project.](https://github.com/kvakulo/Switcheroo)

## Code style

There is a .editorconfig for C# in this project.
Please use it if you Analyze -> code cleanup.

## Create Switcheroo Installer & Portable

1. move into ./Installer
2. run MSBuild.exe Build.xml ([Solution](https://stackoverflow.com/a/13819332/10258204) if MSBuild.exe not found)

You will get an switcheroo-setup.exe & switcheroo-portable.zip in ./Installer/Output

## License

Switcheroo is open source and is licensed under the [GNU GPL v. 3](http://www.gnu.org/licenses/gpl.html).

```
Switcheroo is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Switcheroo is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
 
You should have received a copy of the GNU General Public License
along with Switcheroo.  If not, see <http://www.gnu.org/licenses/>.
```

## Credits

Switcheroo makes use of these open source projects:

* [Managed Windows API](http://mwinapi.sourceforge.net), Copyright © 2006 Michael Schier, GNU Lesser General Public License (LGPL)
* [PortableSettingsProvider](https://github.com/crdx/PortableSettingsProvider), Copyright © crdx, The MIT License (MIT)
