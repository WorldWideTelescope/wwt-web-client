# rc: minor bump

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
