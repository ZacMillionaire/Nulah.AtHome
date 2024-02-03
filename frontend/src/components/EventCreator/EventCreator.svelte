<script lang="ts">
	import { EventStore } from './EventStore';
	import type { NewBasicEventRequest } from './models/NewBasicEventRequest';
	import type { ErrorResponse } from 'models/ErrorResponse';

	let isLoading = false;

	let form: NewBasicEventRequest = {};

	async function createEvent(e: SubmitEvent) {
		isLoading = true;

		await EventStore.CreateEvent(form)
			.then(x => {
				console.log(x);
				isLoading = false;
			})
			.catch((error: Error) => {
				console.log(JSON.parse(error.message) as ErrorResponse[]);
				isLoading = false;
			});
	}
</script>

<form on:submit|preventDefault={createEvent}>
	<input name="name" id="name" bind:value={form.Description} required />
	<input type="datetime-local" name="start" bind:value={form.Start} required />
	<input type="datetime-local" name="end" bind:value={form.End} />
	<input type="text" name="Tags" placeholder="Comma separated tags for this event" bind:value={form.Tags} />
	<button disabled={isLoading}>submit</button>
</form>