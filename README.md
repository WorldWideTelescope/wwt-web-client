# The AAS WorldWide Telescope Web Client

The “web client” of the [AAS](https://aas.org/)
[WorldWide Telescope](http://worldwidetelescope.org/home) (WWT) is a web
application that lets you explore the universe from the comfort of your chair.

### <http://worldwidetelescope.org/webclient>

WWT is a free, open-source tool for visually exploring humanity’s scientific
understanding of the Universe. The web client is the primary portal by which
people interact with WWT — allowing interactive exploration of terabytes of
astronomical data in a seamlessly integrated 4D simulation of the known
universe. However, the WWT software ecosystem also includes a Windows
application that can power planetariums (see [wwt-windows-client]), a
cloud-based web service for discovering and sharing astronomical data (see
[wwt-website]), and [pywwt], a Python module allowing users to write their own
software to control and extend all of these systems.

[wwt-windows-client]: https://github.com/WorldWideTelescope/wwt-windows-client
[wwt-website]: https://github.com/WorldWideTelescope/wwt-website
[pywwt]: https://pywwt.readthedocs.io/

WWT is brought to you by the non-profit [American Astronomical Society] (AAS)
and the [.NET Foundation]. Established in 1899 and based in Washington, DC,
the AAS is the major organization of professional astronomers in North
America.

[American Astronomical Society]: https://aas.org/
[.NET Foundation]: https://dotnetfoundation.org/


## In this repository

This repository contains two major components of WWT:

- The “WebGL engine” that implements the WWT rendering technology as a
  reusable JavaScript library. You can [read its reference documentation]
  and [view standalone code samples].
- The “web client” itself is an [AngularJS](https://angularjs.org/) web app
  that builds on the WebGL engine to provide an immersive experience
  that largely reproduces the experience of [the WWT Windows application].

[read its reference documentation]: https://worldwidetelescope.gitbook.io/webgl-engine-reference/
[view standalone code samples]: http://webhosted.wwt-forum.org/webengine-examples/
[the WWT Windows application]: https://github.com/WorldWideTelescope/wwt-windows-client


## Building the components

Please see [INSTALL.md](./INSTALL.md).


## Getting involved

We love it when people get involved in the WWT community! You can get started
by [participating in our user forum] or by
[signing up for our low-traffic newsletter]. If you would like to help make
WWT better, our [Contributor Hub] aims to be your one-stop shop for
information about how to contribute to the project, with the
[Contributors’ Guide] being the first thing you should read. Here on GitHub we
operate with a standard [fork-and-pull] model.

[participating in our user forum]: https://wwt-forum.org/
[signing up for our low-traffic newsletter]: https://bit.ly/wwt-signup
[Contributor Hub]: https://worldwidetelescope.github.io/
[Contributors’ Guide]: https://worldwidetelescope.github.io/contributing/
[fork-and-pull]: https://help.github.com/en/articles/about-collaborative-development-models

All participation in WWT communities is conditioned on your adherence to the
[WWT Code of Conduct], which basically says that you should not be a jerk.

[WWT Code of Conduct]: https://worldwidetelescope.github.io/code-of-conduct/


## Acknowledgments

The AAS WorldWide Telescope system is a [.NET Foundation] project. Work on WWT
has been supported by the [American Astronomical Society] (AAS), the US
[National Science Foundation] (grants [1550701] and [1642446]), the [Gordon
and Betty Moore Foundation], and [Microsoft].

[.NET Foundation]: https://dotnetfoundation.org/
[National Science Foundation]: https://www.nsf.gov/
[1550701]: https://www.nsf.gov/awardsearch/showAward?AWD_ID=1550701
[1642446]: https://www.nsf.gov/awardsearch/showAward?AWD_ID=1642446
[Gordon and Betty Moore Foundation]: https://www.moore.org/
[Microsoft]: https://www.microsoft.com/


## Legalities

The WWT code is licensed under the [MIT License]. The copyright to the code is
owned by the [.NET Foundation].

[MIT License]: https://opensource.org/licenses/MIT
