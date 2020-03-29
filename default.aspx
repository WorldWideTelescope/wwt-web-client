<!DOCTYPE html>
<!-- -*- html -*- -->
<!-- Some of our APIs redirect to
     worldwidetelescope.org/webclient/default.aspx, passing query string
     parameters. Until we can update the API calls, we need to redirect from
     default.aspx to `./`, honoring the query strings. Unfortunately we
     use a static file server to serve everything within the /webclient/ tree
     and I can't find a way to achieve a server-side redirect for just that
     one URL path. So we have to do it in JavaScript.

     Note also that when we publish this file to our Azure Blob Storage, we
     need to specially set its Content-Type so that it's treated as HTML. -->
<html lang="en">
  <head>
    <meta charset="utf-8">
    <title>Redirect</title>
  </head>
  <body>
    <script>
      window.location.href = "./" + window.location.search;
    </script>
    <p>Redirecting (assuming you have JavaScript enabled) ...</p>
  </body>
</html>
