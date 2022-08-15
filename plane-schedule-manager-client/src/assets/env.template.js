(function(window) {
  window.env = window.env || {};

  // Environment variables
  window["env"]["hubUrl"] = "${HUB_URL}";
  window["env"]["clientId"] = "${CLIENT_ID}";
})(this);