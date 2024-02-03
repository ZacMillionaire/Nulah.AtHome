<script lang="ts">
	import { EventStore } from './EventStore';
	import type { NewBasicEventRequest } from './models/NewBasicEventRequest';
	import type { ErrorResponse } from 'models/ErrorResponse';

	let isLoading = false;

	interface BasicEventRequestForm {
		Description: string;
		Tags: string | null;
		Start: Date;
		End: Date | null;
	}

	let formData: BasicEventRequestForm = {};

	async function createEvent(e: SubmitEvent) {
		isLoading = true;

		let newEvent = {
			Description : formData.Description,
			Start : formData.Start,
			End : formData.End,
			Tags : formData.Tags?.split(",").map(x => x.trim())
		} as NewBasicEventRequest;

		await EventStore.CreateEvent(newEvent)
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
	<input name="name" id="name" bind:value={formData.Description} required />
	<input type="datetime-local" name="start" bind:value={formData.Start} required />
	<input type="datetime-local" name="end" bind:value={formData.End} />
	<input type="text" name="Tags" placeholder="Comma separated tags for this event" bind:value={formData.Tags} />
	<button disabled={isLoading}>submit</button>
</form>