var options = {
    toolbox: document.getElementById('toolbox'),
    collapse: true,
    comments: true,
    disable: true,
    maxBlocks: Infinity,
    trashcan: true,
    horizontalLayout: false,
    toolboxPosition: 'start',
    css: true,
    /*      media : '../media/', */
    rtl: false,
    scrollbars: true,
    sounds: true,
    oneBasedIndex: true,
    zoom: {
        controls: true,
        wheel: true,
        startScale: 1,
        maxcale: 3,
        minScale: 0.3
    },
    grid:
    {
        spacing: 20,
        length: 3,
        colour: '#ccc',
        snap: true
    }
};

var workspace = Blockly.inject('blocklyDiv',options);
var start = document.getElementById("startBlocks");
if (start !=null)
    Blockly.Xml.domToWorkspace(start , workspace);
function loadData(xmlId) {
    
    Blockly.mainWorkspace.clear();
    Blockly.Xml.domToWorkspace(
        document.getElementById(xmlId),workspace);

}   

function loadxml(xml) {
    //debugger;
    if (typeof xml != "string" || xml.length < 5) {
        //alert("No Input");
        return false;
    }
    try {
        var dom = Blockly.Xml.textToDom(xml);
        //window.alert(dom);
        Blockly.mainWorkspace.clear();
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
     showPleaseWait();
     //showCode();
     var xml = Blockly.Xml.workspaceToDom(Blockly.mainWorkspace);
     var xml_text = Blockly.Xml.domToText(xml);
     setCookie("xmlWorkspace", xml_text);
     showCode();
     $("#postData").submit();
 }
 function loadFromCookie() {
     
     loadxml(getCookie("xmlWorkspace"));
 }

 function setCookie(cname, cvalue) {
     if (typeof (Storage) !== "undefined") {
         localStorage.setItem(cname, cvalue);
     }
 }

 function getCookie(cname) {
     if (typeof (Storage) !== "undefined") {
         return localStorage.getItem(cname);
     }
 }