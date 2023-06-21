[![Build Status](https://dev.azure.com/aasworldwidetelescope/WWT/_apis/build/status/WorldWideTelescope.wwt-web-client?branchName=master)](https://dev.azure.com/aasworldwidetelescope/WWT/_build/latest?definitionId=4&branchName=master)
[![npm](https://img.shields.io/npm/v/@wwtelescope/webclient?label=@wwtelescope/webclient)](https://www.npmjs.com/package/@wwtelescope/webclient)
[![Powered by NumFOCUS](https://img.shields.io/badge/powered%20by-NumFOCUS-orange.svg?style=flat&colorA=E1523D&colorB=007D8A)](http://numfocus.org)

# The WorldWide Telescope Web Client

The “web client” of [WorldWide Telescope](http://worldwidetelescope.org/home)
(WWT) is a web application that lets you explore the universe from the comfort
of your chair.

### <https://worldwidetelescope.org/webclient/>

Learn more about WWT [here](https://worldwidetelescope.org/home).

The webclient is an [AngularJS] web app powered by the [WWT WebGL Engine]
JavaScript library.

[AngularJS]: https://angularjs.org/
[WWT WebGL Engine]: https://github.com/WorldWideTelescope/wwt-webgl-engine

[//]: # (numfocus-fiscal-sponsor-attribution)

The WorldWide Telescope project uses an [open governance
model](https://worldwidetelescope.org/about/governance/) and is fiscally
sponsored by [NumFOCUS](https://numfocus.org/). Consider making a
[tax-deductible donation](https://numfocus.org/donate-for-worldwide-telescope)
to help the project pay for developer time, professional services, travel,
workshops, and a variety of other needs.

<div align="center">
  <a href="https://numfocus.org/donate-for-worldwide-telescope">
    <img height="60px"
         src="https://raw.githubusercontent.com/numfocus/templates/master/images/numfocus-logo.png">
  </a>
</div>


## Building and Testing

In order to build and test the app, you need: [Node.js](https://nodejs.org/),
specifically the `npm` command. If you need to install Node.js, use your
operating system’s package manager or visit [nodejs.org](https://nodejs.org/)
for installation instructions.

The first time you check out these files, run:

```
npm install
```

Once that has been done, you can build the website with:

```
npx grunt dist-dev
```

This will create the app files in the `dist` subdirectory of your repository
checkout. To test, all you need is a local HTTP file server pointing at that
subdirectory. We recommend:

```
npx http-server dist
```

This server (and most other static-file servers) will print out a URL that you
can visit to test out the web client locally.

There are also `dist-prod` and `dist-localtest` tasks that configure the build
slightly differently — consult the `profile-*.yml` files, especially
`profile-prod.yml`, to see the parameters that change. By creating a
`profile-localtest.yml` file derived from `profile-dev.yml`, you can monkey
with some low-level settings if you need to do so for testing purposes.


## Deployment

Merges to the `master` branch of this repository will be built and
automatically deployed to the testing version of the webclient:

### <https://worldwidetelescope.org/testing_webclient/>

The production webclient is updated by creating a new release, which is done
using [Cranko] workflows.

[Cranko]: https://pkgw.github.io/cranko/


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

Work on the WorldWide Telescope system has been supported by the [American
Astronomical Society] (AAS), the [.NET Foundation], and other partners. See [the
WWT user website][acks] for details.

[American Astronomical Society]: https://aas.org/
[.NET Foundation]: https://dotnetfoundation.org/
[acks]: https://worldwidetelescope.org/about/acknowledgments/


## Legalities

The WWT code is licensed under the [MIT License]. The copyright to the code is
owned by the [.NET Foundation].

[MIT License]: https://opensource.org/licenses/MIT
