namespace ManagedWinapi.Audio.Mixer {
    /// <summary>
    ///     Represents the method that will handle the <b>LineChanged</b> or
    ///     <b>ControlChanged</b> event of a <see cref="Mixer">Mixer</see>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    ///     A <see cref="MixerEventArgs">MixerEventArgs</see>
    ///     that contains the event data.
    /// </param>
    public delegate void MixerEventHandler(object sender, MixerEventArgs e);
}