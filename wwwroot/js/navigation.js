window.MamiaNav = (function () {
  function initNavigation() {
    const header = document.querySelector('header');
    const menuBtn = document.querySelector('.menu-btn');
    const navAnchors = Array.from(document.querySelectorAll('.nav-links a'));
    const sections = navAnchors
      .map((anchor) => {
        const href = anchor.getAttribute('href') || '';
        if (!href.startsWith('#')) {
          return null;
        }
        return document.querySelector(href);
      })
      .filter(Boolean);

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

    menuBtn.addEventListener('click', function () {
      setMenuState(!header.classList.contains('nav-open'));
    });

    navAnchors.forEach(function (anchor) {
      anchor.addEventListener('click', closeMenu);
    });

    document.addEventListener('keydown', function (event) {
      if (event.key === 'Escape') {
        closeMenu();
      }
    });

    document.addEventListener('click', function (event) {
      if (!header.contains(event.target)) {
        closeMenu();
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
            const href = anchor.getAttribute('href') || '';
            const targetHash = href.includes('#') ? href.split('#').pop() : '';
            const isActive = targetHash === id;
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
