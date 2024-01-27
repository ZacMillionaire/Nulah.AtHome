<script lang="ts">
	import { EventStore } from './EventStore';

	let isLoading = false;

	async function createEvent(e: SubmitEvent) {
		let eventFormData = new FormData(e.target as HTMLFormElement);
		isLoading = true;

		await EventStore.CreateEvent({
			Description: eventFormData.get('name') as string,
			Start: new Date()
		})
			.then(x =>{
				isLoading = false;
			});
	}
</script>

<form on:submit|preventDefault={createEvent}>
	<input name="name" id="name" />
	<button disabled={isLoading}>submit</button>
</form>