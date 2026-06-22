export async function parseApiError(response: Response): Promise<string> {
  try {
    const body = await response.json();
    if (body?.error?.details?.length > 0) {
      return body.error.details.join(' ');
    }
    return body?.error?.message ?? `Request failed (${response.status})`;
  } catch {
    return `Request failed (${response.status})`;
  }
}
