# Building the AAS WorldWide Telescope WebGL Engine and Webclient

As outlined in [the README](./README.md), this repository contains two major
components of WWT:

- The “WebGL engine” that implements the WWT rendering technology as a
  reusable JavaScript library.
- The [AngularJS] “web client” that people experience at
  <http://worldwidetelescope.org/webclient>.

[AngularJS]: https://angularjs.org/

There are essentially two workflows for building the code:

- If you’re on a Windows machine, you can build in Visual Studio.
- Otherwise, you can use our [buildbox] to use a virtual Windows machine to do
  the building through [Vagrant]. This is the recommended workflow for people
  using Linux or macOS.

[buildbox]: https://github.com/WorldWideTelescope/buildbox/
[Vagrant]: https://www.vagrantup.com/

Below we detail these workflows.

The WebGL rendering engine is transpiled from C# into JavaScript using
[ScriptSharp 0.8](https://github.com/nikhilk/scriptsharp/wiki/Release-Notes).
ScriptSharp is no longer under active development but it serves our needs. The
webclient is a standard web app based on [AngularJS](https://angularjs.org/)
and [bootstrap](http://getbootstrap.com). We use the
[angular-strap](http://mgcrea.github.io/angular-strap/) directives library to
integrate the two.


## Windows / Visual Studio build

This repository includes Visual Studio solution files that you can load and
build if you’re running a Windows machine. You’ll need:

- [Visual Studio 2015 Community Edition], which is free. Later versions might
  work, but it is safest to stick with 2015.
- [Node.js], any recent version.

[Visual Studio 2015 Community Edition]: https://visualstudio.microsoft.com/vs/older-downloads/
[Node.js]: https://nodejs.org/en/download/current/

Before you open up this project in Visual Studio, there is a bit of
initialization to do:

1. In a command prompt, navigate to the webclient root and run `npm install`.
2. Install [grunt] with `npm install -g grunt-cli`.
3. Run `grunt watch` from the same command prompt.
4. Disable any LESS compilers you might have as Visual Studio extensions. The
   `grunt watch` will do everything for you.

[grunt]: https://gruntjs.com/

Once that’s done, there are two solutions in this repository that you can choose from:

- `Html5Sdk.sln` builds the WebGL engine (which involves C# transpilation) and the webclient.
- `WebClientOnly.sln` builds the webclient only.

The webclient is essentially a pure JavaScipt app, so its Visual Studio
“build” basically consists of standard [npm] transformations.

[npm]: https://www.npmjs.com/

In Visual Studio, you can right-click the `webclient\default.aspx` file and
select “View in Browser” to test the built webclient locally. To test the
local build of the WebGL engine rather than the production file served off of
`worldwidetelescope.org`, add `?debug=local` to the query string in the
browser you’re using for testing.


## “buildbox” build

If you’re not using Windows, we recommend building using our [Vagrant]-based
[buildbox]. This system will set up a local virtual machine that replicates
the Visual Studio build on your machine.

1. Follow the instructions in [the buildbox README] to set up your Vagrant VM.
   You will have to download about 7 GiB of data, and at the high-water mark
   it will consume about 30 GiB of disk space.
2. Clone this repository *into a subdirectory* of your [buildbox] directory.
3. Use the following commands to perform one-time setup of the tooling that is
   specific to the webclient:
   ```
   ./driver.sh npm install
   ./driver.sh npm install -g grunt-cli
   ./driver.sh nuget install -OutputDirectory ..\\packages
   ```

[the buildbox README]: https://github.com/WorldWideTelescope/buildbox/#one-time-setup

Once this is done, you can build the different project components thusly:

- Use `./driver.sh build-web && ./driver.sh grunt sdk` to generate the WebGL
  engine.
- Use `./driver.sh grunt dist-css dist-js` to compile the webclient assets
- Use `./driver.sh serve-web` to serve the webclient inside the Vagrant box.
  To access it, you must first set up the hostname `MSEDGEWIN10` to be an
  alias to `localhost` on your machine; then you can navigate a browser to
  <http://MSEDGEWIN10:26993/Default.aspx>.


## Testing

We are developing a test suite for the WebGL engine based on [Mocha].
Regardless of which build workflow you’re using, you can run it on your
machine since it’s all based on the portable, generated JavaScript files.

[Mocha]: https://mochajs.org/

To set up the test framework:

1. Navigate to the `tests` subdirectory and run `npm install mocha chai
   mocha-headless-chrome --save-dev`.

To run the tests:

1. In the `tests` subdirectory, run `node_modules/.bin/mocha-headless-chrome
   -f tests.html`
