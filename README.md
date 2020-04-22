[![Build Status](https://dev.azure.com/aasworldwidetelescope/WWT/_apis/build/status/WorldWideTelescope.wwt-web-client?branchName=master)](https://dev.azure.com/aasworldwidetelescope/WWT/_build/latest?definitionId=4&branchName=master)

# The AAS WorldWide Telescope Web Client

The “web client” of the [AAS](https://aas.org/)
[WorldWide Telescope](http://worldwidetelescope.org/home) (WWT) is a web
application that lets you explore the universe from the comfort of your chair.

### <https://worldwidetelescope.org/webclient/>

Learn more about WWT [here](https://worldwidetelescope.org/home).

The webclient is an [AngularJS] web app powered by the [WWT WebGL Engine]
JavaScript library.

[AngularJS]: https://angularjs.org/
[WWT WebGL Engine]: https://github.com/WorldWideTelescope/wwt-webgl-engine


## Building and Testing

In order to build and test the app, you need:

1. [Node.js](https://nodejs.org/), specifically the `npm` command. If you need
   to install Node.js, use your operating system’s package manager or visit
   [nodejs.org](https://nodejs.org/) for installation instructions.
2. The [Grunt](https://gruntjs.com/) task runner, specifically the `grunt`
   command. Once again, install it using your operating system’s package
   manager or [see the Grunt website](https://gruntjs.com/getting-started).

The first time you check out these files, run:

```
npm install
```

Once that has been done, you can build the website with:

```
grunt dist-dev
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
by pushing to this repository a Git tag whose name has the form `vX.Y.Z`. The
version in the `package.json` file should also be updated when doing this.


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

[American Astronomical Society]: https://aas.org/
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
