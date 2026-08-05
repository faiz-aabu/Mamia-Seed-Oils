window.MamiaNav = (function () {
  function initNavigation() {
    const header = document.querySelector('header');
    const menuBtn = document.querySelector('.menu-btn');
    const languageSwitcher = document.querySelector('[data-language-switcher]');
    const languageToggle = document.querySelector('[data-language-toggle]');
    const languageMenu = document.querySelector('[data-language-menu]');
    const navAnchors = Array.from(document.querySelectorAll('.nav-links a[href^="#"]'));
    const sections = navAnchors.map((anchor) => document.querySelector(anchor.getAttribute('href'))).filter(Boolean);

    if (!header || !menuBtn) {
      return;
    }

    const desktopMedia = window.matchMedia('(min-width: 901px)');
    const openLabel = menuBtn.getAttribute('data-open-label') || 'Open menu';
    const closeLabel = menuBtn.getAttribute('data-close-label') || 'Close menu';

    function setMenuState(open) {
      header.classList.toggle('nav-open', open);
      menuBtn.setAttribute('aria-expanded', String(open));
      menuBtn.setAttribute('aria-label', open ? closeLabel : openLabel);
      document.body.style.overflow = open ? 'hidden' : '';
    }

    function closeMenu() {
      if (header.classList.contains('nav-open')) {
        setMenuState(false);
      }
    }

    function closeLanguageMenu() {
      if (!languageToggle || !languageMenu) {
        return;
      }

      languageToggle.setAttribute('aria-expanded', 'false');
      languageMenu.hidden = true;
    }

    function toggleLanguageMenu() {
      if (!languageToggle || !languageMenu) {
        return;
      }

      const isOpen = languageToggle.getAttribute('aria-expanded') === 'true';
      languageToggle.setAttribute('aria-expanded', String(!isOpen));
      languageMenu.hidden = isOpen;
      if (!isOpen) {
        const firstLink = languageMenu.querySelector('a[role="menuitem"]');
        if (firstLink) {
          firstLink.focus();
        }
      }
    }

    menuBtn.addEventListener('click', function () {
      setMenuState(!header.classList.contains('nav-open'));
    });

    if (languageToggle && languageMenu) {
      languageToggle.addEventListener('click', function (event) {
        event.stopPropagation();
        toggleLanguageMenu();
      });
    }

    navAnchors.forEach(function (anchor) {
      anchor.addEventListener('click', closeMenu);
    });

    document.addEventListener('keydown', function (event) {
      if (event.key === 'Escape') {
        closeMenu();
        closeLanguageMenu();
      }
    });

    document.addEventListener('click', function (event) {
      if (!header.contains(event.target)) {
        closeMenu();
      }

      if (languageSwitcher && !languageSwitcher.contains(event.target)) {
        closeLanguageMenu();
      }
    });

    desktopMedia.addEventListener('change', function (event) {
      if (event.matches) {
        setMenuState(false);
      }
    });

    const observer = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (!entry.isIntersecting) {
            return;
          }

          const id = entry.target.id;
          navAnchors.forEach(function (anchor) {
            const isActive = anchor.getAttribute('href') === '#' + id;
            anchor.classList.toggle('is-active', isActive);
            if (isActive) {
              anchor.setAttribute('aria-current', 'page');
            } else {
              anchor.removeAttribute('aria-current');
            }
          });
        });
      },
      { rootMargin: '-40% 0px -50% 0px', threshold: 0.02 }
    );

    sections.forEach(function (section) {
      observer.observe(section);
    });
  }

  return { initNavigation: initNavigation };
})();
