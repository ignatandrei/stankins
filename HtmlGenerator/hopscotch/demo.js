var tour = {
  id: 'hello-hopscotch',
  steps: [
    {
      target: 'theTitle',
      title: 'Welcome to Stankins generator!',
      content: "Let's see how to generate Stankins file",
      placement: 'bottom',
      arrowOffset: 60
    },
	 {
      target: ':1.label',
      title: 'Here you can find jobs!',
      content: "Jobs let you add receivers, transformers, filters and senders\n Click to see data",
      placement: 'bottom',
      arrowOffset: 60
    }
	,
	 {
      target: ':2.label',
      title: 'Here you can find receivers!',
      content: "receivers let's you gather data to be processed",
      placement: 'bottom',
      arrowOffset: 60
    }
	,
	 {
      target: ':3.label',
      title: 'Here you can find transformers!',
      content: "transfomers let's you modify fields name, merge fields, and many more",
      placement: 'bottom',
      arrowOffset: 60
    }
	,
	 {
      target: ':4.label',
      title: 'Here you can find filters!',
      content: "filters let's you fitler the data by values , relations, others\n Click to see data",
      placement: 'bottom',
      arrowOffset: 60
    }
	,
	 {
      target: ':5.label',
      title: 'Here you can find senders!',
      content: "sender let's you send data to various sources",
      placement: 'bottom',
      arrowOffset: 60
    }
	,
	 {
      target: 'showcode',
      title: 'ShowCode',
      content: "After you add receivers, filters , transformers, senders to job press Show Code to generate code",
      placement: 'bottom',
      arrowOffset: 60
    }
	,
	
	 {
      target: 'fileGenerated',
      title: 'Generated file to execute',
      content: "Here you will find the generated file to execute",
      placement: 'left',
      arrowOffset: 60
    }
	,
	 {
      target: ':5.label',
      title: 'Add sender',
      content: "Now click senders and add to the list a sender",
      placement: 'bottom',
      arrowOffset: 60
    }
    
  ],
  showPrevButton: true,
  scrollTopMargin: 100
},

/* ========== */
/* TOUR SETUP */
/* ========== */
addClickListener = function(el, fn) {
  if (el.addEventListener) {
    el.addEventListener('click', fn, false);
  }
  else {
    el.attachEvent('onclick', fn);
  }
},

init = function() {
  var startBtnId = 'startTourBtn',
      calloutId = 'startTourCallout',
      mgr = hopscotch.getCalloutManager(),
      state = hopscotch.getState();

  if (state && state.indexOf('hello-hopscotch:') === 0) {
    // Already started the tour at some point!
    hopscotch.startTour(tour);
  }
  else {
    // Looking at the page for the first(?) time.
    setTimeout(function() {
      mgr.createCallout({
        id: calloutId,
        target: startBtnId,
        placement: 'right',
        title: 'Take an example tour',
        content: 'Start by taking an example tour to see Stankins in action!',
        yOffset: -25,
        arrowOffset: 20,
        width: 240
      });
    }, 100);
  }

  addClickListener(document.getElementById(startBtnId), function() {
    if (!hopscotch.isActive) {
      mgr.removeAllCallouts();
      hopscotch.startTour(tour);
    }
  });
};

init();

