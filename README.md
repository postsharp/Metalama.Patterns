# PostSharp.Engineering.ProductTemplate

This repo is a template for new repos that are built using `PostSharp.Engineering`.

This repo assumes it builds a product named `My.Product`. A _product_, in the context of PostSharp.Engineering, is essentially synonym to a repo, and it can produce several deployable artifacts, typically several NuGet packages. All artifacts in the same product have the same version and build number.

After cloning the repo, you should do this:

* Use `Build.ps1 tools git rename` to rename `My.Product` into your product name, _with_ dots.
* Use Visual Studio Code find-and-replace all to rename:
  * `My.Product` into your product name _with_ dots,
  * `MyProduct` into your product name _without_ dots.

The build entry point is `Build.ps1`.

For more instructions, see https://github.com/postsharp/PostSharp.Engineering.

