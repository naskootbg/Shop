import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";
import { generateInvoicePdfHtml, generatePdfBase64FromHtml } from '@/api/useInvoicePdf';
 
export async function sendMail({
  products,
  coupon,
  total,
  totalCoupon,
  addressDisplay,
  haveTransport,
  haveMail,
  email
}) {
  const html = await generateInvoicePdfHtml({
    products,
    coupon,
    total,
    totalCoupon,
    addressDisplay,
    haveTransport
  });

  const pdfBase64 = await generatePdfBase64FromHtml(html);
  const cleanPdf = pdfBase64.replace(/^data:application\/pdf;base64,/, '');

  const commonPayload = {
    subject: 'WASH-BG.COM - данни за поръчката Ви',
    htmlBody: `Благодарим Ви за поръчката Ви, ${addressDisplay?.fullName || "клиент"}! Вашата фактура е прикачена.`,
    trackingToken: crypto.randomUUID(),
    isSent: false,
    sentAt: null,
    opened: false,
    clicked: false,
    campaignId: null,
    attachments: [
      {
        fileName: 'invoice.pdf',
        content: cleanPdf,
        queuedEmail: null
      }
    ]
  };

  // Always send to support
  await fetch('/api/email/send', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ ...commonPayload, toEmail: "support.bnt@gmail.com" })
  });

  // Send to user if available
  if (haveMail) {
    await fetch('/api/email/send', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ...commonPayload, toEmail: email })
    });
  }
}

export async function GetSettings() {
  try {
    const res = await axiosUrl.get('email/get');
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}


export async function UpdateSettings(smtpServer, smtpPort, smtpUser, smtpPass) {
  try {
    const addon = {
      smtpServer: smtpServer,
      smtpPort: smtpPort,  
      smtpUser: smtpUser,
      smtpPass: smtpPass,
    };

    const res = await axiosUrl.post(`email/update`, addon, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}
export async function justMail(htmlcode, to, subject) {
  if (!Array.isArray(to) || to.length === 0) {
    throw new Error('Recipient list "to" must be a non-empty array');
  }

  const commonPayload = {
    subject,
    htmlBody: htmlcode,
    trackingToken: crypto.randomUUID(),
    isSent: false,
    sentAt: null,
    opened: false,
    clicked: false,
    campaignId: null
  };

  const sendEmailPromises = to.map(email =>
    fetch('/api/email/send', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ...commonPayload, toEmail: email })
    }).then(res => {
      if (!res.ok) {
        throw new Error(`Failed to send email to ${email}: ${res.statusText}`);
      }
      return res.json();
    })
  );

  return Promise.all(sendEmailPromises);
}
export async function AllThemes() {
  try {
    const res = await axiosUrl.get('HtmlTemplates/all');
    return res.data;
  }
  catch (e) {
    logger.error(e);
    return null;
  }
}

export async function ShowTheme(id) {
  try {
    const res = await axiosUrl.get('HtmlTemplates/show?id=' + id);
    return res.data;
  }
  catch (e) {
    logger.error(e);
    return null;
  }
}
export async function EditTheme(id, name, description, htmlContent, orderStatus) {
  try {
    var theme = {
      id: id,
      name: name,
      description: description,
      htmlContent: htmlContent,
      orderStatus: orderStatus
    }
    const res = await axiosUrl.post('HtmlTemplates/edit',
      theme, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });
    if (res.status == 200) {
      return res.data;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}

export async function AddTheme(name, description, htmlContent, orderStatus ) {
  try {
    var theme = {
      name: name,
      description: description,
      htmlContent: htmlContent,
      orderStatus: orderStatus
    }
    const res = await axiosUrl.post('HtmlTemplates/add',
      theme, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });
    if (res.status == 200) {
      return res.data;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}

export async function DelTheme(id) {
  try {
    const res = await axiosUrl.post(`HtmlTemplates/del?id=${id}`,
      {} , {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });
    if (res.status == 200) {
      return res.data;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}
