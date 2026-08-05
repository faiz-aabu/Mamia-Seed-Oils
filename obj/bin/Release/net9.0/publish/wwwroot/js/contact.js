window.MamiaContact = (function () {
  function getCsrfToken() {
    const meta = document.querySelector('meta[name="csrf-token"]');
    return meta ? meta.getAttribute('content') || '' : '';
  }

  async function submitJson(url, payload) {
    const csrf = getCsrfToken();
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-CSRF-TOKEN': csrf
      },
      body: JSON.stringify(payload)
    });

    const contentType = response.headers.get('content-type') || '';
    const body = contentType.includes('application/json') ? await response.json() : null;

    if (!response.ok) {
      const message = body && body.message ? body.message : '';
      throw new Error(message);
    }

    return body;
  }

  function initContact() {
    const contactForm = document.querySelector('[data-contact-form]');
    const newsletterForm = document.querySelector('[data-newsletter-form]');

    if (contactForm) {
      contactForm.addEventListener('submit', async function (event) {
        event.preventDefault();

        const status = contactForm.querySelector('[data-form-status]');
        const submitButton = contactForm.querySelector('button[type="submit"]');
        const submittingMessage = contactForm.getAttribute('data-submitting-message') || '';
        const failedMessage = contactForm.getAttribute('data-failed-message') || '';

        const payload = {
          fullName: (contactForm.querySelector('[name="fullName"]') || {}).value || '',
          companyName: (contactForm.querySelector('[name="company"]') || {}).value || '',
          email: (contactForm.querySelector('[name="email"]') || {}).value || '',
          phone: (contactForm.querySelector('[name="phone"]') || {}).value || '',
          message: (contactForm.querySelector('[name="requirements"]') || {}).value || ''
        };

        if (!contactForm.checkValidity()) {
          contactForm.reportValidity();
          if (status) {
            status.textContent = failedMessage || 'Please review the highlighted fields.';
            status.setAttribute('data-state', 'error');
          }
          return;
        }

        try {
          if (submitButton) {
            submitButton.disabled = true;
          }
          if (status) {
            status.textContent = submittingMessage;
            status.setAttribute('data-state', 'pending');
          }

          const result = await submitJson('/api/contact/enquiry', payload);
          const message = (result && result.message) || contactForm.getAttribute('data-success-message') || '';
          if (status) {
            status.textContent = message;
            status.setAttribute('data-state', 'success');
          }
          contactForm.reset();
        } catch (error) {
          if (status) {
            const errorMessage = (error && error.message) ? error.message : failedMessage;
            status.textContent = errorMessage;
            status.setAttribute('data-state', 'error');
          }
        } finally {
          if (submitButton) {
            submitButton.disabled = false;
          }
        }
      });
    }

    if (newsletterForm) {
      newsletterForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const message = newsletterForm.getAttribute('data-success-message') || '';
        const defaultMessage = newsletterForm.getAttribute('data-default-success-message') || '';
        const finalMessage = message || defaultMessage;
        if (finalMessage) {
          alert(finalMessage);
        }
      });
    }
  }

  return { initContact: initContact };
})();
