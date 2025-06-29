import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";

export async function LoadProducts(hashes) {
  try {
    const res = await axiosUrl.post('order/search-id', {hashes

    }, {
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
