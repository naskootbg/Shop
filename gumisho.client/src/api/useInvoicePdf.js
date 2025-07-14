import html2pdf from 'html2pdf.js';

export async function generateInvoicePdfHtml({
  products,
  coupon,
  couponTotal,
  total,
  addressDisplay,
  haveTransport
}) {
  const container = document.createElement('div');
  const today = new Date().toLocaleDateString('bg-BG');

  const rowsHtml = products.map(item => {
    const name = item.serviceName || 'Без име';
    const size = item.comment || '';
    const area = item.quantityS?.toFixed(2) || '0.00';
    const deliveryPerUnit = haveTransport > 0 ? (item.serviceDelivery || 0) : 0;

    const unitPrice = item.quantityS
      ? ((item.total / item.quantityS) + deliveryPerUnit)
      : 0;

    const lineTotal = (item.total || 0) + deliveryPerUnit * (item.quantityS || 0);

    return `
      <tr>
        <td>${name}</td>
        <td>${size}м</td>
        <td>${area}м²</td>
        <td>${unitPrice.toFixed(2)} лв</td>
        <td>${lineTotal.toFixed(2)} лв</td>
      </tr>
    `;
  }).join('');

  const totalArea = products.reduce((sum, p) => sum + (p.quantityS || 0), 0).toFixed(2);

  const html = `
    <div style="font-family:Arial,sans-serif;padding:20px;max-width:800px;margin:0 auto;">
      <h2 style="text-align:center;">Разписка за поръчка №${products[0]?.orderId || ''}</h2>
      <p><strong>Дата на поръчката:</strong> ${today}</p>

      <table style="width:100%; border-collapse:collapse; margin-top:20px;" border="1" cellpadding="8">
        <thead>
          <tr style="background:#f4f4f4;">
            <th>Име</th>
            <th>Размери</th>
            <th>Квадратура</th>
            <th>Ед. цена</th>
            <th>Общо</th>
          </tr>
        </thead>
        <tbody>
          ${rowsHtml}
        </tbody>
        <tfoot>
          <tr>
    <td colspan="2"><strong>Всичко общо</strong></td>
    <td colspan="2">${totalArea}м²</td>
    <td>${total.toFixed(2)} лв</td>
  </tr>

  ${coupon?.length > 1 ? `
    <tr>
      <td colspan="4"><strong>Промо код: ${coupon}</strong></td>
     <td><strong>${(couponTotal ?? 0).toFixed(2)} лв</strong></td>

    </tr>` : ''}

          <tr>
            <td colspan="3"><h3>Данни за клиента</h3></td>
            <td colspan="2"><h3>Данни за фирмата</h3></td>
          </tr>
          <tr>
            <td colspan="3">Имена: ${addressDisplay?.fullName || ''}</td>
            <td colspan="2">Имена: Вашата Пералня</td>
          </tr>
          <tr>
            <td colspan="3">Телефон: ${addressDisplay?.phone || ''}</td>
            <td colspan="2">Телефон: 0877181618</td>
          </tr>
          <tr>
            <td colspan="3">Адрес: ${addressDisplay?.address || ''}</td>
            <td colspan="2">Адрес: Северна промишлена зона, ул. „Гривишко шосе“, 5803 Плевен</td>
          </tr>
        </tfoot>
      </table>

      <p style="text-align:center; margin-top:20px;">Благодарим за поръчката!</p>
    </div>
  `;

  container.innerHTML = html;
  return container;
}

export async function generatePdfBase64FromHtml(container) {
  const opt = {
    margin: 10,
    filename: 'invoice.pdf',
    html2canvas: { scale: 2 },
    jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
  }

  const blob = await html2pdf().set(opt).from(container).output('blob');

  return await blobToBase64(blob)
}

function blobToBase64(blob) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.onloadend = () => resolve(reader.result.split(',')[1])
    reader.onerror = reject
    reader.readAsDataURL(blob)
  })
}
