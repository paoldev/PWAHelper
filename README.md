# PWAHelper

[![License: MIT](https://img.shields.io/badge/License-MIT-red.svg)](LICENSE.txt)
[![Build and Create Release](https://github.com/paoldev/PWAHelper/actions/workflows/dotnet_create_release.yml/badge.svg)](https://github.com/paoldev/PWAHelper/releases)

Helper tool to create PWA manifest file and icons.

References
* https://w3c.github.io/manifest
* https://json.schemastore.org/web-manifest
* https://developer.mozilla.org/en-US/docs/Web/Manifest

This application saves few common manifest attributes, so pay attention when overwriting an already existing manifest file.  
Supported properties:
* **id**
* **name**
* **short_name**
* **description**
* **lang**
* **start_url**
* **scope**
* **dir**
* **theme_color**
* **background_color**
* **orientation**
* **display**
* **icons**
* **shortcuts**
