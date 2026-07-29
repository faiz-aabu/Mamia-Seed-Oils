window.MamiaAnimations = (function () {
  function initAnimations() {
    const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const hero = document.querySelector('.hero');

    initCounters(reducedMotion);
    initAccordion();
    initRipple();

    if (reducedMotion) {
      return;
    }

    if (hero) {
      window.addEventListener('scroll', function () {
        const offset = Math.min(window.scrollY * 0.12, 40);
        hero.style.setProperty('--hero-parallax', offset + 'px');
      }, { passive: true });
    }

    const revealTargets = document.querySelectorAll('.section-head, .history-timeline .tl-item, .history-copy p, .history-note, .seal, .process-step, .product-card, .gallery-item, .feature-card, .industry-card, .number-tile, .qa-step, .region-card, .faq-item, .testimonial-card, .insight-card, .contact-info .item, .contact-form, .footer-grid-premium > div');

    revealTargets.forEach(function (el, index) {
      el.classList.add('reveal');
      el.style.setProperty('--reveal-delay', String((index % 4) * 55) + 'ms');

      if (el.matches('.history-copy p, .contact-info .item, .feature-card, .testimonial-card')) {
        el.setAttribute('data-reveal', 'left');
      } else if (el.matches('.history-timeline .tl-item, .product-card, .industry-card, .region-card, .insight-card')) {
        el.setAttribute('data-reveal', 'right');
      } else if (el.matches('.number-tile, .qa-step, .gallery-item')) {
        el.setAttribute('data-reveal', 'scale');
      } else {
        el.setAttribute('data-reveal', 'up');
      }
    });

    const revealObserver = new IntersectionObserver(
      function (entries, observer) {
        entries.forEach(function (entry) {
          if (!entry.isIntersecting) {
            return;
          }

          entry.target.classList.add('is-visible');
          observer.unobserve(entry.target);
        });
      },
      { threshold: 0.14, rootMargin: '0px 0px -8% 0px' }
    );

    revealTargets.forEach(function (el) {
      revealObserver.observe(el);
    });
  }

  function initCounters(reducedMotion) {
    const counters = document.querySelectorAll('[data-count]');

    if (reducedMotion) {
      counters.forEach(function (counter) {
        const endValue = Number(counter.getAttribute('data-count'));
        const suffix = counter.getAttribute('data-suffix') || '';
        if (Number.isFinite(endValue)) {
          counter.textContent = endValue.toLocaleString() + suffix;
        }
      });
      return;
    }

    function animateCounter(el) {
      const endValue = Number(el.getAttribute('data-count'));
      const suffix = el.getAttribute('data-suffix') || '';
      if (!Number.isFinite(endValue)) {
        return;
      }

      const duration = 1200;
      const start = performance.now();

      function frame(now) {
        const progress = Math.min((now - start) / duration, 1);
        const eased = 1 - Math.pow(1 - progress, 3);
        const value = Math.floor(endValue * eased);
        el.textContent = value.toLocaleString() + suffix;
        if (progress < 1) {
          requestAnimationFrame(frame);
        }
      }

      requestAnimationFrame(frame);
    }

    const observer = new IntersectionObserver(
      function (entries, io) {
        entries.forEach(function (entry) {
          if (!entry.isIntersecting) {
            return;
          }

          animateCounter(entry.target);
          io.unobserve(entry.target);
        });
      },
      { threshold: 0.4 }
    );

    counters.forEach(function (counter) {
      observer.observe(counter);
    });
  }

  function initAccordion() {
    const accordionItems = Array.from(document.querySelectorAll('[data-accordion] .faq-item'));
    accordionItems.forEach(function (item) {
      const trigger = item.querySelector('.faq-trigger');
      const panel = item.querySelector('.faq-panel');
      if (!trigger || !panel) {
        return;
      }

      trigger.addEventListener('click', function () {
        const isOpen = item.classList.contains('is-open');

        accordionItems.forEach(function (otherItem) {
          const otherTrigger = otherItem.querySelector('.faq-trigger');
          const otherPanel = otherItem.querySelector('.faq-panel');
          otherItem.classList.remove('is-open');
          if (otherTrigger) {
            otherTrigger.setAttribute('aria-expanded', 'false');
          }
          if (otherPanel) {
            otherPanel.style.maxHeight = '0px';
          }
        });

        if (!isOpen) {
          item.classList.add('is-open');
          trigger.setAttribute('aria-expanded', 'true');
          panel.style.maxHeight = panel.scrollHeight + 'px';
        }
      });
    });
  }

  function initRipple() {
    const rippleHosts = document.querySelectorAll('.ripple-host, .btn-primary, .btn-ghost, .nav-cta, form.contact-form button');
    rippleHosts.forEach(function (host) {
      host.classList.add('ripple-host');
      host.addEventListener('pointerdown', function (event) {
        const rect = host.getBoundingClientRect();
        const dot = document.createElement('span');
        dot.className = 'ripple-dot';
        dot.style.left = event.clientX - rect.left + 'px';
        dot.style.top = event.clientY - rect.top + 'px';
        host.appendChild(dot);
        dot.addEventListener('animationend', function () {
          dot.remove();
        });
      });
    });
  }

  return { initAnimations: initAnimations };
})();
