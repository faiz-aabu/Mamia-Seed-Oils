window.MamiaPartnership = (function () {
  function getCsrfToken() {
    const meta = document.querySelector('meta[name="csrf-token"]');
    return meta ? meta.getAttribute('content') || '' : '';
  }

  function initPartnershipForm() {
    const form = document.querySelector('[data-partnership-form]');
    if (!form) {
      return;
    }

    const endpoint = form.getAttribute('data-endpoint') || '/api/partnership/applications';
    const successMessage = form.getAttribute('data-success-message') || '';
    const submittingMessage = form.getAttribute('data-submitting-message') || '';
    const failedMessage = form.getAttribute('data-failed-message') || '';
    const reviewUnavailable = form.getAttribute('data-review-unavailable') || '[To Be Updated]';
    const reviewLabels = {
      companyName: form.getAttribute('data-review-company-name') || 'Company Name',
      businessRegistrationNumber: form.getAttribute('data-review-business-registration-number') || 'Business Registration Number',
      contactPerson: form.getAttribute('data-review-contact-person') || 'Contact Person',
      position: form.getAttribute('data-review-position') || 'Position',
      phoneNumber: form.getAttribute('data-review-phone-number') || 'Phone Number',
      emailAddress: form.getAttribute('data-review-email-address') || 'Email Address',
      businessAddress: form.getAttribute('data-review-business-address') || 'Business Address',
      country: form.getAttribute('data-review-country') || 'Country',
      state: form.getAttribute('data-review-state') || 'State',
      city: form.getAttribute('data-review-city') || 'City',
      businessType: form.getAttribute('data-review-business-type') || 'Business Type',
      yearsInOperation: form.getAttribute('data-review-years-in-operation') || 'Years in Operation',
      monthlyPurchaseEstimate: form.getAttribute('data-review-monthly-purchase-estimate') || 'Monthly Purchase Estimate',
      preferredProducts: form.getAttribute('data-review-preferred-products') || 'Preferred Products',
      preferredPackaging: form.getAttribute('data-review-preferred-packaging') || 'Preferred Packaging',
      additionalNotes: form.getAttribute('data-review-additional-notes') || 'Additional Notes'
    };

    const panels = Array.from(form.querySelectorAll('[data-step-panel]'));
    const indicators = Array.from(form.querySelectorAll('[data-step-indicator]'));
    const backButton = form.querySelector('[data-partnership-back]');
    const nextButton = form.querySelector('[data-partnership-next]');
    const submitButton = form.querySelector('[data-partnership-submit]');
    const status = form.querySelector('[data-partnership-status]');
    const review = form.querySelector('[data-partnership-review]');
    const startedAtField = form.querySelector('[data-form-started-at]');

    if (startedAtField) {
      startedAtField.value = new Date().toISOString();
    }

    let currentStep = 0;

    function collectPayload() {
      const getValue = function (name) {
        const field = form.querySelector('[name="' + name + '"]');
        return field ? (field.value || '').trim() : '';
      };

      const getMultiValue = function (name) {
        const field = form.querySelector('[name="' + name + '"]');
        if (!field || !field.options) {
          return [];
        }

        return Array.from(field.options)
          .filter(function (option) { return option.selected; })
          .map(function (option) { return option.value; });
      };

      const agreedField = form.querySelector('[name="agreedToTerms"]');
      const spamTrapField = form.querySelector('[name="spamTrap"]');
      const startedField = form.querySelector('[name="formStartedAtUtc"]');

      return {
        companyName: getValue('companyName'),
        businessRegistrationNumber: getValue('businessRegistrationNumber'),
        contactPerson: getValue('contactPerson'),
        position: getValue('position'),
        phoneNumber: getValue('phoneNumber'),
        emailAddress: getValue('emailAddress'),
        businessAddress: getValue('businessAddress'),
        country: getValue('country'),
        state: getValue('state'),
        city: getValue('city'),
        businessType: getValue('businessType'),
        yearsInOperation: getValue('yearsInOperation'),
        monthlyPurchaseEstimate: getValue('monthlyPurchaseEstimate'),
        preferredProducts: getMultiValue('preferredProducts'),
        preferredPackaging: getMultiValue('preferredPackaging'),
        additionalNotes: getValue('additionalNotes'),
        agreedToTerms: agreedField ? Boolean(agreedField.checked) : false,
        spamTrap: spamTrapField ? (spamTrapField.value || '') : '',
        formStartedAtUtc: startedField ? startedField.value : ''
      };
    }

    function buildReview() {
      if (!review) {
        return;
      }

      const payload = collectPayload();
      const items = [
        [reviewLabels.companyName, payload.companyName],
        [reviewLabels.businessRegistrationNumber, payload.businessRegistrationNumber || reviewUnavailable],
        [reviewLabels.contactPerson, payload.contactPerson],
        [reviewLabels.position, payload.position || reviewUnavailable],
        [reviewLabels.phoneNumber, payload.phoneNumber],
        [reviewLabels.emailAddress, payload.emailAddress],
        [reviewLabels.businessAddress, payload.businessAddress || reviewUnavailable],
        [reviewLabels.country, payload.country],
        [reviewLabels.state, payload.state],
        [reviewLabels.city, payload.city],
        [reviewLabels.businessType, payload.businessType],
        [reviewLabels.yearsInOperation, payload.yearsInOperation || reviewUnavailable],
        [reviewLabels.monthlyPurchaseEstimate, payload.monthlyPurchaseEstimate],
        [reviewLabels.preferredProducts, payload.preferredProducts.length > 0 ? payload.preferredProducts.join(', ') : reviewUnavailable],
        [reviewLabels.preferredPackaging, payload.preferredPackaging.length > 0 ? payload.preferredPackaging.join(', ') : reviewUnavailable],
        [reviewLabels.additionalNotes, payload.additionalNotes || reviewUnavailable]
      ];

      review.innerHTML = items
        .map(function (entry) {
          return '<div><strong>' + entry[0] + ':</strong> <span>' + escapeHtml(entry[1]) + '</span></div>';
        })
        .join('');
    }

    function escapeHtml(value) {
      return String(value)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
    }

    function panelFields(step) {
      return Array.from(panels[step].querySelectorAll('input, select, textarea')).filter(function (field) {
        return field.type !== 'hidden' && field.name !== 'spamTrap';
      });
    }

    function validateStep(step) {
      const fields = panelFields(step);
      for (let i = 0; i < fields.length; i += 1) {
        const field = fields[i];
        if (!field.checkValidity()) {
          field.reportValidity();
          return false;
        }
      }

      return true;
    }

    function renderStep() {
      panels.forEach(function (panel, index) {
        const isActive = index === currentStep;
        panel.hidden = !isActive;
      });

      indicators.forEach(function (indicator, index) {
        if (index === currentStep) {
          indicator.setAttribute('aria-current', 'step');
        } else {
          indicator.removeAttribute('aria-current');
        }
      });

      if (backButton) {
        backButton.hidden = currentStep === 0;
      }

      if (nextButton) {
        nextButton.hidden = currentStep >= panels.length - 2;
      }

      if (submitButton) {
        submitButton.hidden = currentStep !== panels.length - 1;
      }

      if (currentStep === panels.length - 2) {
        buildReview();
      }
    }

    async function submitForm() {
      const payload = collectPayload();

      if (status) {
        status.textContent = submittingMessage;
        status.setAttribute('data-state', 'pending');
      }

      if (submitButton) {
        submitButton.disabled = true;
      }

      try {
        const response = await fetch(endpoint, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': getCsrfToken()
          },
          body: JSON.stringify(payload)
        });

        const contentType = response.headers.get('content-type') || '';
        const body = contentType.includes('application/json') ? await response.json() : null;

        if (!response.ok) {
          throw new Error(body && body.message ? body.message : failedMessage);
        }

        if (status) {
          status.textContent = (body && body.message) ? body.message : successMessage;
          status.setAttribute('data-state', 'success');
        }

        form.reset();
        currentStep = 0;
        if (review) {
          review.innerHTML = '';
        }
        if (startedAtField) {
          startedAtField.value = new Date().toISOString();
        }
        renderStep();
      } catch (error) {
        if (status) {
          status.textContent = error && error.message ? error.message : failedMessage;
          status.setAttribute('data-state', 'error');
        }
      } finally {
        if (submitButton) {
          submitButton.disabled = false;
        }
      }
    }

    if (nextButton) {
      nextButton.addEventListener('click', function () {
        if (!validateStep(currentStep)) {
          return;
        }

        currentStep = Math.min(currentStep + 1, panels.length - 1);
        renderStep();
      });
    }

    if (backButton) {
      backButton.addEventListener('click', function () {
        currentStep = Math.max(currentStep - 1, 0);
        renderStep();
      });
    }

    form.addEventListener('submit', function (event) {
      event.preventDefault();
      if (!validateStep(currentStep)) {
        return;
      }

      submitForm();
    });

    renderStep();
  }

  return {
    initPartnershipForm: initPartnershipForm
  };
})();
