  var workspace = Blockly.inject('blocklyDiv',
        {media: '../../media/',
         toolbox: document.getElementById('toolbox')});
    Blockly.Xml.domToWorkspace(document.getElementById('startBlocks'),
                               workspace);

function loadxml(xml) {
    debugger;
    if (typeof xml != "string" || xml.length < 5) {
        //alert("No Input");
        return false;
    }
    try {
        var dom = Blockly.Xml.textToDom(xml);
        //window.alert(dom);
        //Blockly.mainWorkspace.clear();
        Blockly.Xml.domToWorkspace(dom, Blockly.mainWorkspace);
        return true;
    } catch (e) {
        alert("Invalid xml" + xml);
        return false;
    }
}
    function showCode() {
      // Generate JavaScript code and display it.
      Blockly.JavaScript.INFINITE_LOOP_TRAP = null;
      var code = Blockly.JavaScript.workspaceToCode(workspace);
	  //debugger;
	  //window.alert(code);
      
	  var x=eval(code);
	  x=transformReceiversSendersTransformer(x);
	  //$("#fileGenerated").val(code);
      $("#fileGenerated").html(orderedStringify(x));
    }
	function transformArray(code, theType){
	var valueType=code[theType];
	if('0' in valueType){
		var value=valueType['0'];
		if(Array.isArray(value))			
		{
			//debugger;
			delete valueType['0'];
			var nr=0;
			for(var i=0;i<value.length;i++){
				var valLoop=value[i];
				if(valLoop == null || valLoop=='null')
						continue;
				valueType[nr++]=valLoop;
			}
			if(nr == 0)
				delete code[theType]
		}
	}
		
	}
 function transformReceiversSendersTransformer(code){
	transformArray(code,"FiltersAndTransformers")
	transformArray(code,"Senders")
	transformArray(code,"Receivers")
	return code;
 }
 function runCode() {
     //showCode();
     //var xml = Blockly.Xml.workspaceToDom(Blockly.Ma);
     //var xml_text = Blockly.Xml.domToText(xml);
     //setCookie("xmlWorkspace", xml_text, 365);

        $("#postData").submit();
 }
 function loadFromCookie() {
     
     //loadxml(getCookie("xmlWorkspace"));
 }

 function setCookie(cname, cvalue, exdays) {
     var d = new Date();
     d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
     var expires = "expires=" + d.toUTCString();
     document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
 }

 function getCookie(cname) {
     var name = cname + "=";
     var ca = document.cookie.split(';');
     for (var i = 0; i < ca.length; i++) {
         var c = ca[i];
         while (c.charAt(0) == ' ') {
             c = c.substring(1);
         }
         if (c.indexOf(name) == 0) {
             return c.substring(name.length, c.length);
         }
     }
     return "";
 }