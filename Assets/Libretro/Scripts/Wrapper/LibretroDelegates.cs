/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using System;
using System.Runtime.InteropServices;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        #region Dynamically loaded function delegates
        // RETRO_API void retro_set_environment(retro_environment_t);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_environment_t(retro_environment_t cb);

        // RETRO_API void retro_set_video_refresh(retro_video_refresh_t);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_video_refresh_t(retro_video_refresh_t cb);

        // RETRO_API void retro_set_audio_sample(retro_audio_sample_t);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_audio_sample_t(retro_audio_sample_t cb);

        // RETRO_API void retro_set_audio_sample_batch(retro_audio_sample_batch_t);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_audio_sample_batch_t(retro_audio_sample_batch_t cb);

        // RETRO_API void retro_set_input_poll(retro_input_poll_t);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_input_poll_t(retro_input_poll_t cb);

        // RETRO_API void retro_set_input_state(retro_input_state_t);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_input_state_t(retro_input_state_t cb);

        // RETRO_API void retro_init(void);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_init_t();

        // RETRO_API void retro_deinit(void);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_deinit_t();

        // RETRO_API unsigned retro_api_version(void);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int retro_api_version_t();

        // RETRO_API void retro_get_system_info(struct retro_system_info *info);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_get_system_info_t(ref retro_system_info info);

        // RETRO_API void retro_get_system_av_info(struct retro_system_av_info *info);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_get_system_av_info_t(ref retro_system_av_info info);

        // RETRO_API void retro_set_controller_port_device(unsigned port, unsigned device);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_controller_port_device_t(uint port, retro_device device);

        // RETRO_API void retro_reset(void);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_reset_t();

        // RETRO_API void retro_run(void);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_run_t();

        // RETRO_API size_t retro_serialize_size(void);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint retro_serialize_size_t();

        // RETRO_API bool retro_serialize(void* data, size_t size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe delegate bool retro_serialize_t(void* data, uint size);

        // RETRO_API bool retro_unserialize(const void* data, size_t size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe delegate bool retro_unserialize_t(void* data, uint size);

        // RETRO_API void retro_cheat_reset(void);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_cheat_reset_t();

        // RETRO_API void retro_cheat_set(unsigned index, bool enabled, const char* code);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void retro_cheat_set_t(uint index, [MarshalAs(UnmanagedType.U1)] bool enabled, char* code);

        // RETRO_API bool retro_load_game(const struct retro_game_info *game);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public delegate bool retro_load_game_t(ref retro_game_info game);

        // RETRO_API bool retro_load_game_special(unsigned game_type, const struct retro_game_info *info, size_t num_info);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public delegate bool retro_load_game_special_t(uint game_type, ref retro_game_info info, uint num_info);

        // RETRO_API void retro_unload_game(void);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_unload_game_t();

        // RETRO_API unsigned retro_get_region(void);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint retro_get_region_t();

        // RETRO_API void* retro_get_memory_data(unsigned id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr retro_get_memory_data_t(uint id);

        // RETRO_API size_t retro_get_memory_size(unsigned id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint retro_get_memory_size_t(uint id);
        #endregion

        #region Mandatory library functions
        // typedef bool (RETRO_CALLCONV *retro_environment_t)(unsigned cmd, void *data);
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe delegate bool retro_environment_t(retro_environment cmd, void* data);

        // typedef void (RETRO_CALLCONV *retro_video_refresh_t)(const void *data, unsigned width, unsigned height, size_t pitch);
        public unsafe delegate void retro_video_refresh_t(void* data, uint width, uint height, uint pitch);

        // typedef void (RETRO_CALLCONV *retro_audio_sample_t)(int16_t left, int16_t right);
        public delegate void retro_audio_sample_t(short left, short right);

        // typedef size_t (RETRO_CALLCONV *retro_audio_sample_batch_t)(const int16_t *data, size_t frames);
        public unsafe delegate uint retro_audio_sample_batch_t(short* data, uint frames);

        // typedef void (RETRO_CALLCONV *retro_input_poll_t)(void);
        public delegate void retro_input_poll_t();

        // typedef int16_t(RETRO_CALLCONV* retro_input_state_t)(unsigned port, unsigned device, unsigned index, unsigned id);
        public delegate short retro_input_state_t(uint port, retro_device device, uint index, uint id);
        #endregion

        #region VFS functions
        // typedef const char*(RETRO_CALLCONV * retro_vfs_get_path_t)(struct retro_vfs_file_handle *stream);
        [return: MarshalAs(UnmanagedType.LPStr)]
        private delegate string retro_vfs_get_path_t(ref retro_vfs_file_handle stream);

        // typedef struct retro_vfs_file_handle *(RETRO_CALLCONV* retro_vfs_open_t) (const char* path, unsigned mode, unsigned hints);
        private delegate ref retro_vfs_file_handle retro_vfs_open_t([MarshalAs(UnmanagedType.LPStr)] string path, uint mode, uint hints);

        // typedef int (RETRO_CALLCONV* retro_vfs_close_t) (struct retro_vfs_file_handle *stream);
        private delegate int retro_vfs_close_t(ref retro_vfs_file_handle stream);

        // typedef int64_t(RETRO_CALLCONV* retro_vfs_size_t)(struct retro_vfs_file_handle *stream);
        private delegate long retro_vfs_size_t(ref retro_vfs_file_handle stream);

        // typedef int64_t(RETRO_CALLCONV* retro_vfs_truncate_t)(struct retro_vfs_file_handle *stream, int64_t length);
        private delegate long retro_vfs_truncate_t(ref retro_vfs_file_handle stream, long length);

        // typedef int64_t(RETRO_CALLCONV* retro_vfs_tell_t)(struct retro_vfs_file_handle *stream);
        private delegate long retro_vfs_tell_t(ref retro_vfs_file_handle stream);

        // typedef int64_t(RETRO_CALLCONV* retro_vfs_seek_t)(struct retro_vfs_file_handle *stream, int64_t offset, int seek_position);
        private delegate long retro_vfs_seek_t(ref retro_vfs_file_handle stream, long offset, int seek_position);

        // typedef int64_t(RETRO_CALLCONV* retro_vfs_read_t)(struct retro_vfs_file_handle *stream, void* s, uint64_t len);
        private delegate long retro_vfs_read_t(ref retro_vfs_file_handle stream, IntPtr s, ulong len);

        // typedef int64_t(RETRO_CALLCONV* retro_vfs_write_t)(struct retro_vfs_file_handle *stream, const void* s, uint64_t len);
        private delegate long retro_vfs_write_t(ref retro_vfs_file_handle stream, IntPtr s, ulong len);

        // typedef int (RETRO_CALLCONV* retro_vfs_flush_t) (struct retro_vfs_file_handle *stream);
        private delegate int retro_vfs_flush_t(ref retro_vfs_file_handle stream);

        // typedef int (RETRO_CALLCONV* retro_vfs_remove_t) (const char* path);
        private delegate int retro_vfs_remove_t([MarshalAs(UnmanagedType.LPStr)] string path);

        // typedef int (RETRO_CALLCONV* retro_vfs_rename_t) (const char* old_path, const char* new_path);
        private delegate int retro_vfs_rename_t([MarshalAs(UnmanagedType.LPStr)] string old_path, [MarshalAs(UnmanagedType.LPStr)] string new_path);

        // typedef int (RETRO_CALLCONV* retro_vfs_stat_t) (const char* path, int32_t *size);
        private delegate int retro_vfs_stat_t([MarshalAs(UnmanagedType.LPStr)] string path, ref int size);

        // typedef int (RETRO_CALLCONV* retro_vfs_mkdir_t) (const char* dir);
        private delegate int retro_vfs_mkdir_t([MarshalAs(UnmanagedType.LPStr)] string dir);

        // typedef struct retro_vfs_dir_handle *(RETRO_CALLCONV* retro_vfs_opendir_t) (const char* dir, bool include_hidden);
        private delegate ref retro_vfs_dir_handle retro_vfs_opendir_t([MarshalAs(UnmanagedType.LPStr)] string dir, [MarshalAs(UnmanagedType.U1)] bool include_hidden);

        // typedef bool (RETRO_CALLCONV* retro_vfs_readdir_t) (struct retro_vfs_dir_handle *dirstream);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_vfs_readdir_t(ref retro_vfs_dir_handle dirstream);

        // typedef const char*(RETRO_CALLCONV * retro_vfs_dirent_get_name_t)(struct retro_vfs_dir_handle *dirstream);
        [return: MarshalAs(UnmanagedType.LPStr)]
        private delegate string retro_vfs_dirent_get_name_t(ref retro_vfs_dir_handle dirstream);

        // typedef bool (RETRO_CALLCONV* retro_vfs_dirent_is_dir_t) (struct retro_vfs_dir_handle *dirstream);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_vfs_dirent_is_dir_t(ref retro_vfs_dir_handle dirstream);

        // typedef int (RETRO_CALLCONV* retro_vfs_closedir_t) (struct retro_vfs_dir_handle *dirstream);
        private delegate int retro_vfs_closedir_t(ref retro_vfs_dir_handle dirstream);
        #endregion

        #region Extra Functions
        // typedef void (RETRO_CALLCONV* retro_set_led_state_t) (int led, int state);
        private delegate void retro_set_led_state_t(int led, int state);

        // typedef bool (RETRO_CALLCONV* retro_midi_input_enabled_t) (void);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_midi_input_enabled_t();
        // typedef bool (RETRO_CALLCONV* retro_midi_output_enabled_t) (void);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_midi_output_enabled_t();
        // typedef bool (RETRO_CALLCONV* retro_midi_read_t) (uint8_t*byte);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_midi_read_t(IntPtr byte_);
        // typedef bool (RETRO_CALLCONV* retro_midi_write_t) (uint8_t byte, uint32_t delta_time);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_midi_write_t(byte byte_, uint delta_time);
        // typedef bool (RETRO_CALLCONV* retro_midi_flush_t) (void);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_midi_flush_t();

        // typedef void (RETRO_CALLCONV *retro_proc_address_t)(void);
        private delegate void retro_proc_address_t();
        // typedef retro_proc_address_t(RETRO_CALLCONV* retro_get_proc_address_t)(const char* sym);
        private delegate retro_proc_address_t retro_get_proc_address_t([MarshalAs(UnmanagedType.LPStr)] string sym);

        // typedef void (RETRO_CALLCONV *retro_log_printf_t)(enum retro_log_level level, const char* fmt, ...);
        public delegate void retro_log_printf_t(retro_log_level level, [MarshalAs(UnmanagedType.LPStr)] string format, IntPtr args);

        // typedef retro_time_t(RETRO_CALLCONV* retro_perf_get_time_usec_t)(void);
        public delegate long retro_perf_get_time_usec_t();
        // typedef retro_perf_tick_t(RETRO_CALLCONV* retro_perf_get_counter_t)(void);
        public delegate ulong retro_perf_get_counter_t();
        // typedef uint64_t(RETRO_CALLCONV* retro_get_cpu_features_t)(void);
        public delegate ulong retro_get_cpu_features_t();
        // typedef void (RETRO_CALLCONV* retro_perf_log_t) (void);
        public delegate void retro_perf_log_t();
        // typedef void (RETRO_CALLCONV* retro_perf_register_t) (struct retro_perf_counter *counter);
        public delegate void retro_perf_register_t(ref retro_perf_counter counter);
        // typedef void (RETRO_CALLCONV* retro_perf_start_t) (struct retro_perf_counter *counter);
        public delegate void retro_perf_start_t(ref retro_perf_counter counter);
        // typedef void (RETRO_CALLCONV* retro_perf_stop_t) (struct retro_perf_counter *counter);
        public delegate void retro_perf_stop_t(ref retro_perf_counter counter);

        // typedef bool (RETRO_CALLCONV* retro_set_sensor_state_t) (unsigned port, enum retro_sensor_action action, unsigned rate);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_set_sensor_state_t(uint port, retro_sensor_action action, uint rate);
        // typedef float (RETRO_CALLCONV* retro_sensor_get_input_t) (unsigned port, unsigned id);
        private delegate float retro_sensor_get_input_t(uint port, uint id);
        // typedef bool (RETRO_CALLCONV* retro_camera_start_t) (void);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_camera_start_t();
        // typedef void (RETRO_CALLCONV* retro_camera_stop_t) (void);
        private delegate void retro_camera_stop_t();
        // typedef void (RETRO_CALLCONV* retro_camera_lifetime_status_t) (void);
        private delegate void retro_camera_lifetime_status_t();
        // typedef void (RETRO_CALLCONV* retro_camera_frame_raw_framebuffer_t) (const uint32_t* buffer, unsigned width, unsigned height, size_t pitch);
        private delegate void retro_camera_frame_raw_framebuffer_t(UIntPtr buffer, uint width, uint height, uint pitch);
        // typedef void (RETRO_CALLCONV* retro_camera_frame_opengl_texture_t) (unsigned texture_id, unsigned texture_target, const float* affine);
        private delegate void retro_camera_frame_opengl_texture_t(uint texture_id, uint texture_target, ref float affine);

        // typedef void (RETRO_CALLCONV* retro_location_set_interval_t) (unsigned interval_ms, unsigned interval_distance);
        private delegate void retro_location_set_interval_t(uint interval_ms, uint interval_distance);
        // typedef bool (RETRO_CALLCONV* retro_location_start_t) (void);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_location_start_t();
        // typedef void (RETRO_CALLCONV* retro_location_stop_t) (void);
        private delegate void retro_location_stop_t();
        // typedef bool (RETRO_CALLCONV* retro_location_get_position_t) (double* lat, double* lon, double* horiz_accuracy, double* vert_accuracy);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_location_get_position_t(ref double lat, ref double lon, ref double horiz_accuracy, ref double vert_accuracy);
        // typedef void (RETRO_CALLCONV* retro_location_lifetime_status_t) (void);
        private delegate void retro_location_lifetime_status_t();

        // typedef bool (RETRO_CALLCONV* retro_set_rumble_state_t) (unsigned port, enum retro_rumble_effect effect, uint16_t strength);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_set_rumble_state_t(uint port, retro_rumble_effect effect, ushort strength);

        // typedef void (RETRO_CALLCONV *retro_audio_callback_t)(void);
        private delegate void retro_audio_callback_t();
        // typedef void (RETRO_CALLCONV *retro_audio_set_state_callback_t)(bool enabled);
        private delegate void retro_audio_set_state_callback_t([MarshalAs(UnmanagedType.U1)] bool enabled);

        // typedef void (RETRO_CALLCONV* retro_frame_time_callback_t) (retro_usec_t usec);
        private delegate void retro_frame_time_callback_t(long usec);

        // typedef void (RETRO_CALLCONV* retro_hw_context_reset_t) (void);
        private delegate void retro_hw_context_reset_t();
        // typedef uintptr_t(RETRO_CALLCONV* retro_hw_get_current_framebuffer_t)(void);
        private delegate UIntPtr retro_hw_get_current_framebuffer_t();
        // typedef retro_proc_address_t(RETRO_CALLCONV* retro_hw_get_proc_address_t)(const char* sym);
        private delegate retro_proc_address_t retro_hw_get_proc_address_t([MarshalAs(UnmanagedType.LPStr)] string sym);

        // typedef void (RETRO_CALLCONV* retro_keyboard_event_t) (bool down, unsigned keycode, uint32_t character, uint16_t key_modifiers);
        private delegate void retro_keyboard_event_t([MarshalAs(UnmanagedType.U1)] bool down, uint keycode, uint character, ushort key_modifiers);

        // typedef bool (RETRO_CALLCONV* retro_set_eject_state_t) (bool ejected);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_set_eject_state_t(bool ejected);
        // typedef bool (RETRO_CALLCONV* retro_get_eject_state_t) (void);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_get_eject_state_t();
        // typedef unsigned(RETRO_CALLCONV* retro_get_image_index_t)(void);
        private delegate uint retro_get_image_index_t();
        // typedef bool (RETRO_CALLCONV* retro_set_image_index_t) (unsigned index);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_set_image_index_t(uint index);
        // typedef unsigned(RETRO_CALLCONV* retro_get_num_images_t)(void);
        private delegate uint retro_get_num_images_t();
        // typedef bool (RETRO_CALLCONV* retro_replace_image_index_t) (unsigned index, const struct retro_game_info *info);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_replace_image_index_t(uint index, ref retro_game_info info);
        // typedef bool (RETRO_CALLCONV* retro_add_image_index_t) (void);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_add_image_index_t();
        // typedef bool (RETRO_CALLCONV* retro_set_initial_image_t) (unsigned index, const char* path);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_set_initial_image_t(uint index, [MarshalAs(UnmanagedType.LPStr)] string path);
        // typedef bool (RETRO_CALLCONV* retro_get_image_path_t) (unsigned index, char* path, size_t len);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_get_image_path_t(uint index, [MarshalAs(UnmanagedType.LPStr)] string path, uint len);
        // typedef bool (RETRO_CALLCONV* retro_get_image_label_t) (unsigned index, char* label, size_t len);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool retro_get_image_label_t(uint index, [MarshalAs(UnmanagedType.LPStr)] string label, uint len);
        #endregion
    }
}
