(function () {
	function init() {
		document.querySelectorAll('img').forEach(function (img) {
			if (!img.hasAttribute('loading')) {
				img.setAttribute('loading', 'lazy');
			}
			if (!img.hasAttribute('decoding')) {
				img.setAttribute('decoding', 'async');
			}
		});

		if (window.MamiaNav) {
			window.MamiaNav.initNavigation();
		}

		if (window.MamiaAnimations) {
			window.MamiaAnimations.initAnimations();
		}

		if (window.MamiaGallery) {
			window.MamiaGallery.initGallery();
		}

		if (window.MamiaContact) {
			window.MamiaContact.initContact();
		}

		if (window.MamiaPartnership) {
			window.MamiaPartnership.initPartnershipForm();
		}

		if (window.MamiaAssistant) {
			window.MamiaAssistant.initAssistant();
		}
	}

	if (document.readyState === 'loading') {
		document.addEventListener('DOMContentLoaded', init);
	} else {
		init();
	}
})();
