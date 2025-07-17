import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";
        
export async function Subscribe(email, userName, tag, category, productCode, maxPrice, minPercent ) {
    try {
        const res = await axiosUrl.post('subscribe/sub', {
           // push,
            email,
            userName,
            tag,
            category,
            productCode,
            maxPrice,
            minPercent
        }, {
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

export async function MySubscriptions(email) {
    try {
        const res = await axiosUrl.post('subscribe/my?email=' + email, {
             
        }, {
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