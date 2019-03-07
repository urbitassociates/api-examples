const throwIfNotOk = response => {
  if (response.ok) return;

  return response.json().then(body => {
    console.error("Error!", response, body);
  });
};

module.exports = {
  throwIfNotOk
};
