export const throwIfNotOk = async (response) => {
  if (response.ok) return;

  const body = await response.json();
  console.error("Error", response.status, body);
  throw new Error("Error calling API");
};
