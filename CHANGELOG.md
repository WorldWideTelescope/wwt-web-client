# @wwtelescope/webclient 6.4.3 (2023-11-02)

- Fix recovery of share links based on constellation indices (#363, @Carifio24).


# @wwtelescope/webclient 6.4.2 (2023-06-21)

- Update sponsorship branding


# @wwtelescope/webclient 6.4.1 (2023-02-15)

- Make sliders in layer properties window be initialized with the correct value
  (#354, @Carifio24).


# @wwtelescope/webclient 6.4.0 (2022-07-12)

- Finally update this to expose all of our new datasets (@pkgw, #353)! These
  have been managed in the [wwt-core-catalogs][wcc] repository, but not actually
  exposed in the production webclient until now. The update required us to
  rebuild the webclient’s precompiled search index, which took a lot of
  infrastructural work. That is now working.
- Fix a formatting bug that could lead to the seconds value being 60 (@Carifio24, #351).

[wcc]: https://github.com/WorldWideTelescope/wwt-core-catalogs/


# @wwtelescope/webclient 6.3.5 (2022-05-19)

- index.html: load pako and uuid early for updated webgl-engine

# @wwtelescope/webclient 6.3.4 (2022-02-15)

- WWT 2022 is officially launched!

# @wwtelescope/webclient 6.3.3 (2022-02-10)

- Tease the WWT 2022 launch.

# @wwtelescope/webclient 6.3.2 (2021-11-12)

- Add donation links in a few places (#346, @pkgw).

# @wwtelescope/webclient 6.3.1 (2021-04-13)

- Improve catalog HiPS UX: it now ought to work just to click on the thumbnails.

# @wwtelescope/webclient 6.3.0 (2021-04-07)

- Add SOFIA Studies to the default webclient data collection
- Make it possible to explore Catalog HiPS data, although the UX is unpolished —
  you basically need to be a developer to know what to do

# @wwtelescope/webclient 6.2.3 (2021-02-17)

- Work on display of planetary imagery: more reliable loading from WTML
  and Explore ribbon thumbnail clicks, hopefully (#338, @pkgw)
- Fix the saving of colors picked from the color picker; should enable
  people to create transparent overlays in the tour editor (#333, @pkgw)
- UI updates to support HiPS menu items! (#334, @imbasimba)

# @wwtelescope/webclient 6.2.2 (2020-11-21)

- Attempt to fix the OAuth login flow (#332)
- Hide the Layer Manager by default (#331)
- Construct better URLs for "research" queries (#330)

# @wwtelescope/webclient 6.2.1 (2020-10-12)

- Increase precision of RA readouts (#328, #326)

# @wwtelescope/webclient 6.2.0 (2020-10-12)

- Update the URL for the hosted engine JavaScript: `engine/7` as opposed to
  `engine/7.4`. So we will now pick up engine updates more regularly.
- Start using Cranko for release automation.
- Attempt to fix #316
