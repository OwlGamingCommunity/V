var g_InputEnabled = false;
function SetInputEnabled(enabled)
{
  g_InputEnabled = enabled;
}

document.onkeydown = function()
{
  if (!IsRunningInRAGECEF())
  {
    return true;
  }

  if (!g_InputEnabled)
  {
    return false;
  }

  if(window.event && (window.event.keyCode == 9 || window.event.keyCode == 13))
  {
    return false; 
  }
}

function IsRunningInRAGECEF()
{
  var isRageCEF = false;

  if (navigator.deviceMemory == null || navigator.userAgent.indexOf("Windows NT 6.2") != -1 || navigator.userAgent.indexOf("Chrome/70.0.3538.77") != -1)
  {
    isRageCEF = true;
  }

  return isRageCEF;
}

var tagBody = '(?:[^"\'>]|"[^"]*"|\'[^\']*\')*';

var tagOrComment = new RegExp(
    '<(?:'
    // Comment body.
    + '!--(?:(?:-*[^->])*--+|-?)'
    // Special "raw text" elements whose content should be elided.
    + '|script\\b' + tagBody + '>[\\s\\S]*?</script\\s*'
    + '|style\\b' + tagBody + '>[\\s\\S]*?</style\\s*'
    // Regular name
    + '|/?[a-z]'
    + tagBody
    + ')>',
    'gi');

function removeTags(str)
{

  if (str != null)
  {
    if (typeof str === 'string' || str instanceof String)
    {
      var oldStr;
      do
      {
        oldStr = str;
        str = str.replace(tagOrComment, '');
        str = str.replace(/`/g, '')
      } while (str !== oldStr);

      return str.replace(/</g, '&lt;');
    }
  }

  return str;
}

function TriggerEvent(name, argsVardiadic)
{
  if (!IsRunningInRAGECEF())
  {
    console.log(`RAGE EVENT TRIGGERED: ${name}`, arguments);
    return;
  }
  
  var trueLength = arguments.length - 1;

  for (var i = 1; i < trueLength + 1; ++ i)
  {
    arguments[i] = removeTags(arguments[i]);
  }

  if (trueLength == 0) { mp.trigger("UI", name); }
  else if (trueLength == 1) { mp.trigger("UI", name, arguments[1]); }
  else if (trueLength == 2) { mp.trigger("UI", name, arguments[1], arguments[2]); }
  else if (trueLength == 3) { mp.trigger("UI", name, arguments[1], arguments[2], arguments[3]); }
  else if (trueLength == 4) { mp.trigger("UI", name, arguments[1], arguments[2], arguments[3], arguments[4]); }
  else if (trueLength == 5) { mp.trigger("UI", name, arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]); }
  else if (trueLength == 6) { mp.trigger("UI", name, arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]); }
  else if (trueLength == 7) { mp.trigger("UI", name, arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7]); }
  else if (trueLength == 8) { mp.trigger("UI", name, arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8]); }
  else if (trueLength == 9) { mp.trigger("UI", name, arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9]); }
  else if (trueLength == 10) { mp.trigger("UI", name, arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10]); }
  else
  {
    console.log(`ERROR: Too many args. Unsupported (${trueLength})`);
  }
}

window.onload = function ()
{
  if (typeof OnLoad === "function")
  {
      OnLoad();
  }
  
  if (typeof AddDebugData === "function")
  {
    //AddDebugData();
  }

  var guiID = window.location.href.substring(window.location.href.lastIndexOf('#') + 1);
  document.title = guiID;
    TriggerEvent("OnHTMLLoaded", guiID, window.location.href);
}

function animateCSS(element, animationName, callback) {
    const node = document.querySelector(element)
    node.classList.add('animated', animationName)

    function handleAnimationEnd() {
        node.classList.remove('animated', animationName)
        node.removeEventListener('animationend', handleAnimationEnd)

        if (typeof callback === 'function') callback()
    }

    node.addEventListener('animationend', handleAnimationEnd)
}